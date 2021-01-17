using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RentHelper.Database;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public class ProductRepository : IProductRepository
    {

        private readonly AppDbContext _context;
        
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        //public bool isProductExist(Guid productId)
        //{
        //    return _context.products.Any(p => p.Id == productId);
        //}

        public Task<int> addProduct(Product product,List<Picture> pics) 
        {
            return Task<int>.Run(()=> {
                if (product.Address == null || product.Address == "") //若未輸入地址 則從使用者取得
                {
                    var usr = _context.users.FirstOrDefault(u => u.Id == product.UserId);
                    product.Address = (usr != null) ? usr.Address : "";
                }
                product.createTime = DateTime.Now;
                var tmp = _context.products.Add(product);
                save();//需先存入一次 才會讓資料庫產生 pid

                var tmpProduct = _context.products.FirstOrDefault(p => p == product);
                if(tmpProduct==null){ return -1; }//產品新增失敗
                var pid = tmpProduct.Id;
                product.Id = pid;

                //圖片處理
                var flag = false;
                if (pics.Count() > 0)
                {
                    flag = true;
                    for (int i = 0; i < pics.Count; i++)
                    {
                        pics[i].ProductId = pid; //存入對應資訊
                        pics[i].DateTime = DateTime.Now; //存入產生時間

                        if (pics[i].Path.StartsWith("http"))
                        {
                            _context.pictures.Add(pics[i]); //加到資料庫
                            continue; //若已經是路徑檔案 則不做儲存
                        }
                        pics[i].Path = picSave(pics[i].Path, i, pid);
                        if (pics[i].Path != "")
                        {
                            _context.pictures.Add(pics[i]); //加到資料庫
                        }
                    }
                }

                if (flag) { save(); }

                return _context.products.Any(p => p == product) ? 0 : -1; //確認已建立且存在 返回0 若建立失敗 返回-1
            });
        }
        private string picSave(string Base64Str, int i, Guid productId)
        {
            string fileName = productId.ToString() + String.Format("_{0}", i) + ".jpg";
            string dirPath = Directory.GetCurrentDirectory() + "/Image/";
            string savePath = dirPath + fileName;
           
            string NetPath = @"http://35.221.140.217/image/" + fileName;

            string data = Base64Str
                .Replace("data:image/png;base64,", "")
                .Replace("data:image/jgp;base64,", "")
                .Replace("data:image/jpg;base64,", "")
                .Replace("data:image/jpeg;base64,", "");  //要處理下字符串 ,之前的要截取掉 不然會報錯

            byte[] arr;
            
            try
            {
                arr = Convert.FromBase64String(data);}
            catch (Exception)
            {
                return "";//圖片異常 回傳空字串 會排除該圖片物件
                //return "Convert fail!error:" + e.Message;
            }

            try
            {
                using MemoryStream ms = new MemoryStream(arr);
                string[] picList = Directory.GetFiles(dirPath, "*.jpg");
                foreach (string f in picList)
                {
                    if (f[(dirPath.Length + 1)..] == fileName) { File.Delete(f); }//檔名相同 先刪除
                }
                Bitmap bmp = new Bitmap(ms);
                bmp = ResizeBitmap(bmp, 512);
                bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Close();
            }
            catch (Exception)
            {
                return "";
                //return "Save Fail! error:" +e.Message;
            }
            
            return NetPath;
        }
        public Bitmap ResizeBitmap(Bitmap bmp, int max)
        {
            //取得圖片長寬
            int bmpW = bmp.Width;
            int bmpH = bmp.Height;
            if (bmpW <= max && bmpH <= max) { return bmp; }
            decimal scale = (bmpH > bmpW) ? bmpH / max : bmpW / max;
            bmpW = (int)(bmpW / scale);
            bmpH = (int)(bmpH / scale);
            Bitmap result = new Bitmap(bmpW, bmpH);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, bmpW, bmpH);
            }
            return result;
        }


        public Task<Product> getProduct(Guid productId)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p=>p.createTime)
                .Include(p => p.Pics) //積極式載入 
                .FirstOrDefault(p => p.Id == productId && (p.isRent || p.isSale || p.isExchange) && p.amount >0));
        }

        public Task<Product> getMyProduct(Guid userId, Guid productId) {

            return Task.FromResult(_context.products
                .Include(p => p.Pics) //包含外鍵列表的取得
                .FirstOrDefault(p => p.Id == productId && p.UserId==userId));
        }

        public Task<IEnumerable<Product>> myOwnList(Guid userId)
        {

            var plist = _context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics) //包含外鍵列表的取得
                .Where(p => p.UserId == userId).ToList();

            plist.Reverse();

            return Task.FromResult<IEnumerable<Product>>( plist);
        }


        public Task<IEnumerable<Product>> myOwnListNotOnShelf(Guid userId)
        {
            return Task.FromResult<IEnumerable<Product>>(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics) //包含外鍵列表的取得
                .Where(p => 
                    p.UserId == userId &&  (!(p.isRent || p.isSale || p.isExchange) || p.amount == 0)
                    ).ToList());
        }

        public Task<IEnumerable<Product>> myOwnListOnShelf(Guid userId)
        {
            return Task.FromResult< IEnumerable < Product >> (_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics) //包含外鍵列表的取得
                .Where(p =>
                    p.UserId == userId && (p.isRent || p.isSale || p.isExchange) && p.amount > 0
                    ).ToList());
        }

        public Task<IEnumerable<Product>> myOwnListByOrderStatusBuyer(Guid userId,String status)
        {
            return Task.FromResult<IEnumerable<Product>>(_context.orders
                .OrderByDescending(o => o.OrderTime)
                .Where(o => o.Lender == userId && (o.status.getStatusDesc() == status || status == "All"))
                .Join(_context.products,
                    o => o.ProductId, p => p.Id,
                    (o, p) => p));//交集兩張表，輸出符合的product資料
        }
        public Task<IEnumerable<Product>> myOwnListByOrderStatusSeller(Guid userId, String status)
        {
            return Task.FromResult<IEnumerable<Product>>(_context.orders
                .OrderByDescending(o => o.OrderTime)
                .Where(o => o.p_ownerId == userId && ( o.status.getStatusDesc() == status || status == "All"))
                .Join(_context.products,
                    o => o.ProductId, p => p.Id,
                    (o, p) => p));//交集兩張表，輸出符合的product資料
        }

        public Task<IEnumerable<Product>> listBy(string type, string type1, string type2, string index, string length)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => 
                    (p.Type == type || type =="All" ) &&
                    (p.Type1 == type1 || type1 == "All") &&
                    (p.Type2 == type2 || type2 == "All") &&
                    (p.isRent || p.isSale || p.isExchange) && p.amount > 0)//可售出項目
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }


        public Task<IEnumerable<Product>> listByBrand(string type, string index, string length)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.Type == type && (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }


        public Task<IEnumerable<Product>> listByBrandNType2(string type , string type2 , string index , string length )
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.Type == type && p.Type2== type2 &&
                (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }

        public Task<IEnumerable<Product>> listByType1(string type1, string index, string length)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.Type1 == type1 && (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }

        public Task<IEnumerable<Product>> listByType2(string type1, string type2, string index,string length)
        {
            return Task.FromResult( _context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.Type1 == type1 && p.Type2 == type2 && (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index)-1).Take(int.Parse(length)));
        }

        public Task<IEnumerable<Product>> listByKeyword(string type1, string type2, string keyword, string index, string length)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.Type1 == type1 && p.Type2 == type2 && p.Title.Contains(keyword) && (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }

        public Task<IEnumerable<Product>> listBySeller(Guid SellerId, string index, string length)
        {
            return Task.FromResult(_context.products
                .OrderByDescending(p => p.createTime)
                .Include(p => p.Pics)
                .Where(p => p.UserId== SellerId && (p.isRent || p.isSale || p.isExchange) && p.amount > 0)
                .ToList().Skip(int.Parse(index) - 1).Take(int.Parse(length)));
        }

        public Task<int> productDelete(Guid productId,Guid ownerId)
        {
            return Task<int>.Run(()=> {
                var product = _context.products.FirstOrDefault(p => p.Id == productId && p.UserId == ownerId);
                if (product == null)
                {
                    return -2;//無此id 無法操作 返回-2
                }

                var olist = _context.orders.Where(o => o.ProductId == productId);
                if (olist != null && olist.Count() > 0) {
                    olist.ForEachAsync(o => {
                        o.Product = null;
                        o.ProductId = null;
                    });
                    _context.orders.UpdateRange(olist);
                }

                var wlist = _context.cartitems.Where(ci => ci.ProductId == productId);
                if (wlist != null && wlist.Count() > 0) {
                    _context.cartitems.RemoveRange(wlist);
                }

                _context.products.Remove(product);
                save(); //需要執行 資料庫更新
                return _context.products.Any(p => p.Id == productId) ? -1 : 0; //確定查不到id 返回0 若還存在 返回-1刪除失敗
            });
        }

        public Task<int> productUpdate(Product product, Guid ownerId)
        {
            return Task<int>.Run(()=> {
                if (!_context.products.Any(p => p.Id == product.Id && p.UserId == ownerId))
                {
                    return -2;//無符合條件產品
                }

                var pics = product.Pics.ToList();
                product.Pics.Clear();//圖片需要另外處理

                product.UserId = ownerId;//強迫寫入使用者ID
                product.createTime = DateTime.Now;//寫入更新時間
                _context.products.Update(product);

                if (product.Address == "") //若未輸入地址 則從使用者取得
                {
                    var usr = _context.users.FirstOrDefault(u => u.Id == product.UserId);
                    product.Address = (usr != null) ? usr.Address : "";
                }

                //圖片處理
                var tmpPicList = _context.pictures.Where(pic => pic.ProductId == product.Id).ToList();//先取出 準備要移除用
                                                                                                      //取得原有圖片庫 編號最大值
                var picNumber = 0;
                var itmp = 0;
                tmpPicList.ForEach(pic => {
                    if (!int.TryParse(pic.Path.Split("_")[1].Split(".")[0], out itmp)) { itmp = 0; }
                    picNumber = (itmp > picNumber) ? itmp : picNumber;
                });

                pics.ForEach(pic => {//pic 送過來的
                    var tmpPic = tmpPicList.FirstOrDefault(tmpP => tmpP.Id == pic.Id);
                    if (tmpPic != null)
                    {
                        tmpPicList.Remove(tmpPic);
                    }
                });
                _context.pictures.RemoveRange(tmpPicList); //移除不在更新資訊的圖片

                //加入新圖片
                for (int i = 0; i < pics.Count; i++)
                {
                    if (_context.pictures.Any(p => p.Id == pics[i].Id)) { continue; }

                    pics[i].ProductId = product.Id; //存入對應資訊
                    pics[i].DateTime = DateTime.Now; //存入產生時間
                    if (pics[i].Path == null) { continue; }

                    if (pics[i].Path.StartsWith("http"))
                    {
                        _context.pictures.Add(pics[i]); //加到資料庫
                        continue; //若已經是路徑檔案 則不做儲存
                    }
                    pics[i].Path = picSave(pics[i].Path, i + picNumber + 1, product.Id);
                    if (pics[i].Path != "")
                    {
                        _context.pictures.Add(pics[i]); //加到資料庫
                    }
                }
                save();
                return 0;
            });
            
        }

        public IEnumerable<Picture> getPicturesByProductId(Guid productId)
        {
            return _context.pictures
                .Where(p => p.ProductId == productId).ToList();
        }

        public int save()
        {
            return _context.SaveChanges(); //需要執行 資料庫更新
        }

        public void PicClr()
        {
            string dirPath = Directory.GetCurrentDirectory() + "/Image/";
            string[] picList = Directory.GetFiles(dirPath, "*.jpg");
            foreach (string f in picList)
            {
                string fName = f[dirPath.Length..];
                if (!_context.pictures.Any(p => p.Path == "http://35.221.140.217/image/" + fName)) 
                {
                    File.Delete(f); 
                }//不在清單內
            }
        }

    }
}
