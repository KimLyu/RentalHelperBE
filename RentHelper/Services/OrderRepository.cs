using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using RentHelper.Database;
using RentHelper.Dtos;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public int addOrderQueue(Order order, List<OrderExchangeItem> oeItems) 
        {
            if (!_context.users.Any(u => u.Id == order.Lender)) { return -2; }//確認為有效使用者

            Product product = _context.products.FirstOrDefault(p => p.Id == order.ProductId);//取出購買產品的資料
            if (product == null) { return -3; }//確認產品ID有效
            if (order.TradeQuantity <= 0 || order.TradeQuantity > product.amount)
            { return -4; }//交易數量 與商品數量的確認

            //進行快照
            order.p_Title = product.Title;
            order.p_Desc = product.Description;
            order.p_Address = product.Address;
            order.p_isSale = product.isSale;
            order.p_isRent = product.isRent;
            order.p_isExchange = product.isExchange;
            order.p_Deposit = product.Deposit;
            order.p_Rent = product.Rent;
            order.p_salePrice = product.salePrice;
            order.p_RentMethod = product.RentMethod;
            order.p_Type = product.Type;
            order.p_Type1 = product.Type1;
            order.p_Type2 = product.Type2;
            order.p_ownerId = product.UserId;
            order.p_WeightPrice = product.WeightPrice;

            //確認交易模式
            if ((order.TradeMethod == 0 && !product.isRent) ||
                (order.TradeMethod == 1 && !product.isSale) ||
                (order.TradeMethod == 2 && !product.isExchange))
            { return -8; }//訂單交易模式錯誤

            //處理交換項目
            if (product.isExchange && order.TradeMethod == 2)
            {
                //確認交換數量檢查 及 權重價值判定
                decimal oeItemWeight = 0;
                bool QtyErrFlag = false;
                foreach (var item in oeItems)
                {
                    var wishItem = _context.wishItems
                        .FirstOrDefault(wi => wi.Id == item.WishItemId && wi.UserId == product.UserId);//檢查願望清單 必須屬於產品擁有者
                    if (wishItem == null) { return -7; }
                    if (wishItem.RequestQuantity < item.packageQuantity)
                    {
                        QtyErrFlag = true;//數量檢查有異常
                    }
                    oeItemWeight += item.packageQuantity * wishItem.WeightPoint;
                }
                if (product.WeightPrice > oeItemWeight) { return -5; }//權重價值不符
                if (QtyErrFlag) { return -6; } //超過需求 不符合
            }
            //檢查完 先儲存order 並取得oid
            _context.orders.Add(order);
            save();

            var oid = _context.orders.FirstOrDefault(o => o == order).Id;
            order.Id = oid;//回存 可讓Controller取得
            if (product.isExchange && order.TradeMethod == 2)
            {
                //加入oid連結後，加入資料庫 同時扣除願望清單數量
                oeItems.ForEach(item => {
                    item.OrderId = oid;
                    var wishItem = _context.wishItems.FirstOrDefault(wi => wi.Id == item.WishItemId);
                    wishItem.RequestQuantity -= item.packageQuantity * order.TradeQuantity;
                    _context.wishItems.Update(wishItem);
                });
                _context.orderExchangeItems.AddRange(oeItems);
            }

            //產品數量需扣除
            product.amount -= order.TradeQuantity;
            _context.products.Update(product);
            //產品從購物車移除
            var ci = _context.cartitems.FirstOrDefault(ci => ci.ProductId == order.ProductId && ci.UserId == order.Lender);
            if (ci != null)
            {
                _context.cartitems.Remove(ci);
            }
            save();

            return _context.orders.Any(o => o.Id == oid) ? 0 : -1;

        }

        public Task<int> addOrder(Order order, List<OrderExchangeItem> oeItems)
        {
            return Task<int>.Run(()=> {
                return addOrderQueue( order, oeItems);
            });
        }

        public Task<IEnumerable<Order>> getMyOrders(Guid uid) //自己全部訂單
        {
            return Task.FromResult< IEnumerable < Order >>( _context.orders
                .Include(o => o.notes)
                .Include(o => o.OrderExchangeItems)
                    .ThenInclude(oeItem => oeItem.WishItem)
                .Include(o=>o.Product)
                    .ThenInclude(p=>p.Pics)//DTO需要使用 目前必須加
                .Where(o => o.Lender == uid || o.p_ownerId == uid));
        }

        public Task<IEnumerable<Order>> buyList(Guid lenderId,String status) //購入
        {
            var arr = Enum.GetValues(typeof(OrderStatus));
            OrderStatus sta = OrderStatus.NONE;
            foreach (OrderStatus item in arr) 
            {
                if (item.getStatusDesc() == status) { sta = item; }
            }

            return Task.FromResult<IEnumerable<Order>>( _context.orders
                .Include(o => o.notes)
                .Include(o => o.OrderExchangeItems)
                    .ThenInclude(oeItem => oeItem.WishItem)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Pics)//DTO需要使用 目前必須加
                .Where(o => o.Lender == lenderId && (o.status == sta || status=="All")).ToList());
        }

        public Task<IEnumerable<Order>> sellList(Guid ownerId, String status) //售出
        {
            var arr = Enum.GetValues(typeof(OrderStatus));
            OrderStatus sta = OrderStatus.NONE;
            foreach (OrderStatus item in arr)
            {
                if (item.getStatusDesc() == status) { sta = item; }
            }
            return Task.FromResult < IEnumerable < Order >> (_context.orders
                .Include(o => o.notes)
                .Include(o => o.OrderExchangeItems)
                    .ThenInclude(oeItem => oeItem.WishItem)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Pics)//DTO需要使用 目前必須加
                .Where(o => o.p_ownerId == ownerId && (o.status == sta || status == "All")).ToList());
        }


        public Task<Order> getOrderById(Guid ownerId,Guid OrderId) //單張
        {
            return Task.FromResult(_context.orders
                .Include(o => o.notes)
                .Include(o => o.OrderExchangeItems)
                    .ThenInclude(oeItem => oeItem.WishItem)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Pics)//DTO需要使用 目前必須加
                .FirstOrDefault(o => (o.Lender==ownerId || o.p_ownerId== ownerId) && o.Id== OrderId));
        }

        

        public bool isOrderExist(Guid OrderId)
        {
            return _context.orders.Any(o => o.Id == OrderId);
        }

        public Task<int> OrderDelete(Guid uid, Guid OrderId)
        {
            return Task<int>.Run(()=>
            {
                if (!_context.users.Any(u => u.Id == uid)) { return -2; }
                //尚未寄出貨物之前 只允許賣家進行刪除
                var order = _context.orders.FirstOrDefault(o => o.Id == OrderId);
                if (order == null) { return -3; }
                if (order.p_ownerId != uid) { return -4; }//賣家才有權限刪除
                                                          //<TODO> 這邊還要考慮金流狀態 若已付款 不可刪除 

                if (order.status.getStatusDesc() != "已立單") { return -5; } //尚未寄出貨物前 才能刪除


                _context.orders.Remove(order);
                save();

                return _context.orders.Any(o => o.Id == OrderId) ? -1 : 0;
            });
        }

        public int save()
        {
            return _context.SaveChanges(); //需要執行 資料庫更新
        }

        public Task<int> orderStatusChange(Guid userId,Guid orderId, string nowStatus,OrderStatusDto OrderStatusDto)
        {
            return Task<int>.Run(()=> {
                var order = _context.orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null) { return -6; }//無效訂單id
                if (!_context.users.Any(u => u.Id == userId)) { return -5; } //無效使用者id
                if (order.Lender != userId && order.p_ownerId != userId) { return -5; }

                OrderStatus preStatus = OrderStatus.NONE;
                OrderStatus nextStatus = OrderStatus.NONE;
                var statusArray = Enum.GetValues(typeof(OrderStatus));
                foreach (OrderStatus status in statusArray)
                {
                    if (status.getStatusDesc() == nowStatus) 
                    { 
                        preStatus = status;
                    }
                }
                switch (preStatus) {
                    case OrderStatus.CREATE:
                        nextStatus = OrderStatus.SEND;
                        break;
                    case OrderStatus.SEND:
                        nextStatus = OrderStatus.ARRIVED;
                        break;
                    case OrderStatus.ARRIVED:
                        nextStatus = OrderStatus.SENDBACK;
                        break;
                    case OrderStatus.SENDBACK:
                        nextStatus = OrderStatus.GETBACK;
                        break;
                }
                Console.WriteLine(
                    "Input Status = " + preStatus + 
                    "\nNext Status = "+ nextStatus +
                    "\nOrder Status = " + order.status
                    );
                if (preStatus == OrderStatus.NONE || nextStatus == OrderStatus.NONE
                    || (int)preStatus >= (int)nextStatus)
                { return -4; }//狀態設定不正確
                if (order.status != preStatus) { return -3; }//非目前狀態

                bool errFlag = false;
                var newTime = DateTime.Now;
                switch (nextStatus)
                {
                    case OrderStatus.PAYED:
                        order.PayTime = newTime;
                        if (order.Lender != userId) { errFlag = true; }
                        break;
                    case OrderStatus.SEND:
                        order.ProductSend = newTime;
                        if (order.p_ownerId != userId) { errFlag = true; }
                        break;
                    case OrderStatus.ARRIVED:
                        order.ProductArrive = newTime;
                        if (order.Lender != userId) { errFlag = true; }
                        break;
                    case OrderStatus.SENDBACK:
                        order.ProductSendBack = newTime;
                        if (order.Lender != userId) { errFlag = true; }
                        break;
                    case OrderStatus.GETBACK:
                        order.ProductGetBack = newTime;
                        if (order.p_ownerId != userId) { errFlag = true; }
                        break;
                    default:
                        errFlag = true;
                        break;
                }
                if (errFlag) { return -2; }//操作錯誤

                order.status = nextStatus;
                if ((order.TradeMethod == 1 && nextStatus == OrderStatus.ARRIVED) ||
                    (order.TradeMethod != 1 && nextStatus == OrderStatus.GETBACK))
                {
                    order.status = OrderStatus.CLOSE;
                }


                OrderStatusDto.NewStatus = nextStatus.getStatusDesc();
                OrderStatusDto.DateTime = newTime.ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo);


                _context.orders.Update(order);
                save();
                return _context.orders.Any(o => o == order) ? 0 : -1;
            });
            
        }

        public async void firebasePushNotifaction(Guid uid,Guid oid, OrderStatusDto oStatusDto)
        {
            var odr = _context.orders.FirstOrDefault(o => o.Id == oid);
            if (odr == null) { return; }
            Guid tmpUsrId = new Guid();
            if (uid == odr.Lender) { tmpUsrId = odr.p_ownerId; }
            else if (uid == odr.p_ownerId) { tmpUsrId = (Guid)odr.Lender; }
            else { return; }//異常則不發送

            var usr = _context.users.FirstOrDefault(u => u.Id == tmpUsrId);
            if (usr == null) { return; }

            var token = usr.dTK;//需編碼加密 取得後解開
            Debug.WriteLine("token:" + token);
            if (token == null || token == "") { return; }
            // This registration token comes from the client FCM SDKs.
            var registrationToken = token;

            // See documentation on defining a message payload.
            var message = new Message
            {
                Data = new Dictionary<string, string>()
                {
                    { "noteType","status" },
                    { "ProductTitle",odr.p_Title},
                    { "OrderId", odr.Id.ToString() },
                    { "newStatus", oStatusDto.NewStatus },
                    { "newStatusTime", oStatusDto.DateTime }
                },
                Token = registrationToken
            };


            // Send a message to the device corresponding to the provided
            // registration token.
            try
            {
                string response = await FirebaseMessaging.DefaultInstance
                    .SendAsync(message);

                // Response is a message ID string.
                Debug.WriteLine("Successfully sent message: " + response);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error meaasge: " + e.Message);
            }


        }
    }
}
