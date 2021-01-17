using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class OrderExchangeItemDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid WishItemId { get; set; }
        
        //快照資訊
        public string ExchangeItem { get; set; } //訂單選擇的交換內容
        public int packageQuantity { get; set; } //訂單交易的交換數量 
    }
}
