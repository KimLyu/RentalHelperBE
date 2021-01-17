using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }//產品名稱
        public string Description { get; set; }//產品說明
        public string Address { get; set; }//產品位於何處
        public bool isSale { get; set; } //設定 是否出售
        public bool isRent { get; set; } //設定 是否出租
        public bool isExchange { get; set; } //設定 是否以物易物
        public int Deposit { get; set; }//訂金
        public int Rent { get; set; }//日租金
        public int salePrice { get; set; }//售價
        public string RentMethod { get; set; }//交易方式說明
        public decimal amount { get; set; } //還可交易數量
        public string Type { get; set; } // Playstation / xbox / switch 桌游
        public string Type1 { get; set; } //ps5/ps4/xbox serial/xbox one/switch/table/other 
        public string Type2 { get; set; } //主機/遊戲/硬體周邊/其他

        //<表單關聯-父表>
        public Guid UserId { get; set; }

        //<表單關聯-子表>產品持有多張圖片
        public ICollection<PictureDto> Pics { get; set; }

        public decimal WeightPrice { get; set; } //交換時 需要多少個WeightPoint可交易此產品

        public string createTime { get; set; }//創建時間
    }
}
