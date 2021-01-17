using Microsoft.EntityFrameworkCore;
using RentHelper.Database;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using BC = BCrypt.Net.BCrypt;

namespace RentHelper.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool isUserExist(string email)
        {
            return _context.users.FirstOrDefault(u => u.Email == email) != null;
        }

        public Task<int> addUser(User user,string verifycode)
        {
            return Task<int>.Run(()=> {
                if (user.Email == null || _context.users.Any(u => u.Email == user.Email))
                {
                    return -2; //email應該為uniqe 重複建立返回-2
                }
                var vCode = _context.verifycodes.FirstOrDefault(v => v.Email == user.Email && v.VerityCode == verifycode);
                if (vCode == null)
                {
                    return -4; //尚未進行信件確認
                }
                if (DateTime.Now.Subtract(vCode.CreateTime).Minutes >= 30)
                {
                    return -3;//email確認超時
                }

                //使用者密碼 須加密
                //var salt = "";//字串調用
                user.Password = BC.HashPassword(user.Password);

                _context.users.Add(user);
                _context.verifycodes.Remove(vCode);
                save();
                return isUserExist(user.Email) ? 0 : -1; //確認已建立且存在 返回0 若建立失敗 返回-1

            });
            
        }

        public Task<Guid?> checkUser(string email, string pwd,string dTK)
        {
            
            var usr = _context.users.FirstOrDefault(u => u.Email == email);
            if (usr == null || !BC.Verify(pwd, usr.Password))
            {
                return Task.FromResult<Guid?>(null);
            }

            if (dTK != "")
            {
                usr.dTK = dTK;
                _context.users.Update(usr);
                save();
            }
            return Task.FromResult<Guid?>(usr.Id);
            
        }
        public Task<User> getUserBase(Guid userId)
        {
            return Task<User>.Run(()=> {
                var usr = _context.users
                .Include(u => u.Products)
                .Include(u => u.WishItems)
                .FirstOrDefault(usr => usr.Id == userId);
                if (usr != null && usr.WishItems != null && usr.WishItems.Count() > 0)
                {
                    usr.WishItems = usr.WishItems.Where(wi => wi.RequestQuantity > 0).ToList();
                }
                return usr;
            });
        }

        public Task<User> getUser(Guid userId)
        {
            return Task<User>.Run(()=> {
                return _context.users
                    .Include(u => u.Products)
                    .Include(u => u.WishItems)
                    .FirstOrDefault(usr => usr.Id == userId);
            });
        }

        public Task<int> userUpdate(User user)
        {
            return Task<int>.Run(()=> {
                var usr = _context.users
                .FirstOrDefault(u => u.Id == user.Id && u.Email == user.Email);
                if (usr == null)
                {
                    return -2;//無符合條件產品
                }
                _context.users.Update(user);//尚未確認
                _context.SaveChanges(); //需要執行 資料庫更新

                return 0;
            });
            
        }
        public Task<int> userDelete(Guid userId)
        {
            return Task<int>.Run(()=> {
                var usr = _context.users.FirstOrDefault(u => u.Id == userId);
                if (usr == null) { return -2; } //無此id 無法操作 返回-2
                _context.users.Remove(usr);
                _context.SaveChanges(); //需要執行 資料庫更新
                return _context.users.Any(u => u.Id == userId) ? -1 : 0; //確定查不到id 返回0 若還存在 返回-1刪除失敗
            });
        }

        public Task<int> sendEmailcheck(string email) //使用Gmail SMTP發信
        {
            return Task<int>.Run(()=> {
                if (!email.Contains("@"))
                {
                    return -2;//沒@符號非email
                }
                string verifyCode = genVerifyCode(8);
                _context.verifycodes.RemoveRange(_context.verifycodes.Where(v => v.Email == email));
                _context.verifycodes.Add(new EmailVerifyCode(email, verifyCode));
                SendAutomatedEmail(email, "<h1> verifyCode : " + verifyCode + "</h1>");
                _context.SaveChanges();

                return _context.verifycodes.Any(v => v.Email == email && v.VerityCode == verifyCode) ? 0 : -1;
            });
        }
        private string genVerifyCode(int length) {

            Random rnd = new Random();
            string[] str = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                    "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                    "0","1","2","3","4","5","6","7","8","9"};
            
            string randomstr = "";

            for (int i = 0; i < length; i++)
            {
                randomstr += str[rnd.Next(str.Length)];
            }
            return randomstr;
        }
        private void SendAutomatedEmail(string ReceiveMail, string Body)
        {
            MailMessage MyMail = new MailMessage();
            MyMail.From = new MailAddress("RentalHelperManager@gmail.com");
            MyMail.To.Add(ReceiveMail); //設定收件者Email
            //MyMail.Bcc.Add("goustx@gmail.com"); //加入密件副本的Mail          
            MyMail.Subject = "RentHelper Email test";
            MyMail.SubjectEncoding = System.Text.Encoding.Unicode;
            MyMail.Body = Body;//設定信件內容
            MyMail.IsBodyHtml = true; //是否使用html格式
            SmtpClient MySMTP = new SmtpClient("smtp.gmail.com", 587);
            MySMTP.Credentials = new System.Net.NetworkCredential("RentalHelperManager@gmail.com", "backend@rental");
            MySMTP.EnableSsl = true;
            try
            {
                MySMTP.Send(MyMail);
                MyMail.Dispose(); //釋放資源
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Task<int> userPwdByVerify(string Email, string VerifyCode, string newPwd)
        {
            return Task<int>.Run(()=> {
                var usr = _context.users.FirstOrDefault(u => u.Email == Email);
                if (usr == null) { return -2; }//無此使用者
                var vCode = _context.verifycodes.FirstOrDefault(v => v.Email == Email && v.VerityCode == VerifyCode);
                if (vCode == null) { return -4; } //尚未進行信件確認
                if (DateTime.Now.Subtract(vCode.CreateTime).Minutes >= 30) { return -3; } //email確認超時

                //使用者密碼 須加密
                //var salt = "";//字串調用
                usr.Password = BC.HashPassword(newPwd);

                _context.users.Update(usr);
                _context.verifycodes.Remove(vCode);
                save();
                return _context.users.Any(u => u.Email == Email && u.Password == usr.Password) ? 0 : -1;
            });
            
        }
        public Task<int> userPwdChange(Guid usrID,string oldPwd, string newPwd)
        {
            return Task<int>.Run(()=> {
                var usr = _context.users.FirstOrDefault(u => u.Id == usrID);
                if (usr == null) { return -2; } //無此ID
                if (usr.Password != oldPwd) { return -1; } // 密碼錯誤
                usr.Password = BC.HashPassword(newPwd);
                _context.users.Update(usr);
                save();
                return _context.users.Any(u => u.Id == usrID && u.Password == usr.Password) ? 0 : -1;
            });
        }
        public int save()
        {
            return _context.SaveChanges(); //需要執行 資料庫更新
        }

        public Task<int> addWish(Guid uid, WishItem wishItem)
        {
            return Task<int>.Run(()=> {
                if (!_context.users.Any(u => u.Id == uid)) { return -3; } //無效使用者
                if (wishItem.ExchangeItem == "" || wishItem.ExchangeItem == null || wishItem.RequestQuantity <= 0 || wishItem.WeightPoint <= 0)
                { return -2; } //資料錯誤
                wishItem.UserId = uid;
                _context.wishItems.Add(wishItem);
                save();
                
                var tmpWi = _context.wishItems.FirstOrDefault(wi => wi.UserId == wishItem.UserId && wi.ExchangeItem == wishItem.ExchangeItem);
                if (tmpWi != null) 
                { 
                    wishItem.Id = tmpWi.Id;
                    return 0;
                }
                return -1;
            });
        }

        public Task<IEnumerable<WishItem>> getWishList(Guid uid)
        {
            return Task.FromResult<IEnumerable<WishItem>>(_context.wishItems.Where(wi => wi.UserId == uid));
        }
        public Task<IEnumerable<WishItem>> getWishListOnShelf(Guid uid)
        {
            return Task.FromResult<IEnumerable<WishItem>>(_context.wishItems
                .Where(wi => wi.UserId == uid && wi.RequestQuantity > 0));
        }

        public Task<int> delWishById(Guid uid, Guid wishItemId)
        {
            return Task<int>.Run(()=> {
                if (!_context.users.Any(u => u.Id == uid)) { return -4; } //無效使用者
                var wishItem = _context.wishItems.FirstOrDefault(wi => wi.Id == wishItemId);
                if (wishItem == null) { return -3; } //無效願望清單
                if (wishItem.UserId != uid) { return -2; } //無權限刪除
                _context.wishItems.Remove(wishItem);
                save();
                return _context.wishItems.Any(wi => wi.Id == wishItem.Id) ? -1 : 0;
            });
        }

        
    }
}
