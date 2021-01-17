using FirebaseAdmin.Messaging;
using RentHelper.Database;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public class NoteRepository: INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> addNote(Guid uid, Note note)
        {
            return Task<int>.Run(()=> {
                //檢查傳入的Notes內容
                if (!_context.users.Any(u => u.Id == uid)) { return -5; }//無效使用者
                var order = _context.orders.FirstOrDefault(o => o.Id == note.OrderId);
                if (order == null) { return -4; };//無此訂單
                if (!(order.Lender == uid || order.p_ownerId == uid)) { return -3; }//非關聯者
                //if (_context.notes.Any(n =>
                //    n.SenderId == uid &&
                //    n.Message == note.Message &&
                //    n.OrderId == note.OrderId))
                //{ return -2; }//重複發送
                note.SenderName = _context.users.FirstOrDefault(u => u.Id == uid).Name;
                _context.notes.Add(note);
                save();
                var n = _context.notes.FirstOrDefault(n => n == note);
                if (n != null) { note.Id = n.Id; }

                return (n != null) ? 0 : -1;
            });
        }

        public async void firebasePushNotifaction(Note note)
        {

            var tmpOrder = _context.orders.FirstOrDefault(o => o.Id == note.OrderId);
            if (tmpOrder == null) { return; }

            
            Guid tmpUsrId = new Guid();
            if (note.SenderId == tmpOrder.Lender) { tmpUsrId = tmpOrder.p_ownerId; }
            else if (note.SenderId == tmpOrder.p_ownerId) { tmpUsrId = (Guid)tmpOrder.Lender; }
            else { return; }//異常則不發送

            var usr = _context.users.FirstOrDefault(u => u.Id == tmpUsrId);
            if (usr == null) { return; }

            var token = usr.dTK;//需編碼加密 取得後解開
            //token = "dfd2HVXueEKEqeVAUrAEjb:APA91bF-x10hr9ui2h6RNHQev-Y60dclTVStMIImTnrm4KMGVXKoyIBRZKu5SjFMX4l9VahxaHwbnSrYsb5k7C56xtNIYHips-GWQbO6BM1V3KMGPqby_haaeSyy9OoCAxIppiexNObH";//ios測試
            //token = "f9udJQH4ST2TBLtPFYacz7:APA91bGnZxPnniL97rhjudHfbfZdLx_CAWLE6iFXWqFsirt-n-odZTPOQQjFSF4YAQpokfzxeQ5lAl43t-aojsw-1JYL1F4SSWVXq0zNDRDQy6VDatpvHuOdIAjlrF9dTLdmqkVuAJ7M";//android
            Debug.WriteLine("token:"+ token);
            if (token == null || token == "") { return; }
            // This registration token comes from the client FCM SDKs.
            var registrationToken = token;

            // See documentation on defining a message payload.
            var message = new Message
            {
                Data = new Dictionary<string, string>()
                {
                    { "noteType","message" },
                    { "ProductTitle",tmpOrder.p_Title},
                    { "Sender", note.SenderName },
                    { "OrderId", note.OrderId.ToString() },
                    { "Message", note.Message }
                },
                Token = registrationToken
            };


            // Send a message to the device corresponding to the provided
            // registration token.
            try {
                string response = await FirebaseMessaging.DefaultInstance
                    .SendAsync(message);

                // Response is a message ID string.
                Debug.WriteLine("Successfully sent message: " + response);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Error meaasge: " + e.Message);
            }
            

        }

        public Task<IEnumerable<Note>> getList(Guid uid,Guid oid)
        {
            List<Note> nlist = new List<Note>();
            
                var order = _context.orders.FirstOrDefault(o =>
                    o.Id == oid &&
                    (o.Lender == uid || o.p_ownerId == uid));

            if (order == null) { nlist = null; }
            else
            {
                nlist.AddRange(_context.notes
                .Where(n => n.OrderId == oid));
            }
           
            return Task.FromResult<IEnumerable<Note>>(nlist);
        }

        public Task<int> delNote(Guid uid, Guid nid)
        {
            return Task<int>.Run(()=> {
                if (!_context.users.Any(u => u.Id == uid)) { return -4; }//無效使用者id
                var note = _context.notes.FirstOrDefault(n => n.Id == nid);
                if (note == null) { return -3; }//查無此Note
                if (note.SenderId != uid) { return -2; }//無權限
                _context.notes.Remove(note);
                save();
                return _context.notes.Any(n => n.Id == nid) ? -1 : 0;
            });
        }

        public int save()
        {
            return _context.SaveChanges(); //需要執行 資料庫更新
        }

        
    }
}
