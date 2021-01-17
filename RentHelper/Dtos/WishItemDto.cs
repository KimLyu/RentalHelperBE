using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class WishItemDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string ExchangeItem { get; set; } //希望可換得的內容

        public int RequestQuantity { get; set; } //需求數量

        public decimal WeightPoint { get; set; }//每一個的權重數值
    }
}
