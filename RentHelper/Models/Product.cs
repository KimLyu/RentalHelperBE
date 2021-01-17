using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class Product
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //引入上方Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }//產品名稱
        public string Description { get; set; }//產品說明
        public string Address { get; set; }//產品位於何處
        [Required]
        public bool isSale { get; set; } //設定 是否出售
        [Required]
        public bool isRent { get; set; } //設定 是否出租
        [Required]
        public bool isExchange { get; set; }//設定 是否以物易物
        public int Deposit { get; set; }//訂金
        public int Rent { get; set; }//日租金
        public int salePrice { get; set; }//售價
        [Required]
        public string RentMethod { get; set; }//交易方式說明
        [Required]
        public decimal amount { get; set; } //還可交易數量
        [Required]
        public string Type { get; set; } // Playstation / xbox / switch 桌游
        [Required]
        public string Type1 { get; set; } // ps5/ps4 /xbox serial/xbox one /switch/ table/other 
        [Required]
        public string Type2 { get; set; } //主機/遊戲/硬體周邊/其他

        //<表單關聯-父表>
        [ForeignKey("UserId")]//外鍵連結關係 會自動將主鍵([Key]的Id)與類名(Product)關聯
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }//反向導覽

        //<表單關聯-子表>產品持有多張圖片
        public ICollection<Picture> Pics { get; set; }
            = new List<Picture>();


        public decimal WeightPrice { get; set; } //交換時 需要多少個WeightPoint可交易此產品

        public DateTime createTime { get; set; }//創建時間

    }
}
