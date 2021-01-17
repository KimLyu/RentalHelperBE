using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        //產品快照資料
        public string p_Title { get; set; }//產品名稱
        public string p_Desc { get; set; }//產品說明
        public string p_Address { get; set; }//產品位於何處
        public bool p_isSale { get; set; } //設定 是否出售
        public bool p_isRent { get; set; } //設定 是否出租
        public bool p_isExchange { get; set; }//設定 是否以物易物
        public int p_Deposit { get; set; }//訂金
        public int p_Rent { get; set; }//日租金
        public int p_salePrice { get; set; }//售價
        public string p_RentMethod { get; set; }//交易方式說明
        public string p_Type { get; set; } // Playstation / xbox / switch 桌游
        public string p_Type1 { get; set; } //ps5/ps4/xbox serial/xbox one/switch/table/other 建議改用enum
        public string p_Type2 { get; set; } //主機/遊戲/硬體周邊/其他
        public Guid p_ownerId { get; set; }//產品賣家
        public decimal p_WeightPrice { get; set; } //交換時 需要多少個WeightPoint可交易此產品
        //產品快照資料

        //產品圖片 插入用
        public ICollection<PictureDto> Pics { get; set; }
            = new List<PictureDto>();

        //交易選擇資料
        public int TradeMethod { get; set; } //0:租用 1:販售 2:以物易物
        public IEnumerable<OrderExchangeItemDto> OrderExchangeItems { get; set; }//交易用的選項表單
        public int TradeQuantity { get; set; }//交易數量

        //訂單狀態
        public string status { get; set; }//訂單狀態 (暫存、已成立、已寄出、已取得、歸還寄出、已歸還、封存)
        public string OrderTime { get; set; }//下單時間
        public string PayTime { get; set; }//付款時間
        public string ProductSend { get; set; }//貨物寄交
        public string ProductArrive { get; set; }//確認取得
        public string ProductSendBack { get; set; }//歸還寄交
        public string ProductGetBack { get; set; }//物件返回時間

        //訂單關係
        public Guid ProductId { get; set; }//交易的產品 紀錄用 不做查詢 用為連結，但可能被刪除下架。另上方新增快照資料。
        public Guid Lender { get; set; }//紀錄租借人(自己) 不做外鍵關聯 若刪除帳號 不影響訂單紀錄
        //若上方兩筆資料都已被刪除，須定時檢查並清除訂單紀錄。

        public ICollection<NoteDto> notes { get; set; }
            = new List<NoteDto>();
    }
}
