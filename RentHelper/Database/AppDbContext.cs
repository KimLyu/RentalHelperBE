using Microsoft.EntityFrameworkCore;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Database
{
    public class AppDbContext : DbContext
    {
        //代碼與資料庫的聯繫
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        //數據模型映射
        public DbSet<Picture> pictures { get; set; }//產品圖的表單
        public DbSet<User> users { get; set; }//使用者表單
        public DbSet<EmailVerifyCode> verifycodes { get; set; }//存放驗證碼表單
        //暫存Email對應VerifyCode 以及記錄生成時間 
        //使用者註冊時需要輸入驗證碼 作為查詢用

        public DbSet<Product> products { get; set; }//產品表單
        public DbSet<Order> orders { get; set; }//訂單表單
        public DbSet<OrderExchangeItem> orderExchangeItems { get; set; }//訂單儲存的交換物件
        public DbSet<CartItem> cartitems { get; set; }//購物車表單
        public DbSet<Note> notes { get; set; }//通知表單

        public DbSet<WishItem> wishItems { get; set; } //產品的 願望物品 表單
    //    protected override void OnModelCreating(ModelBuilder modelBulider)
    //    {
    //        modelBulider.Entity<Order>()


    //    }
    }
}
