using RentHelper.Database;
using RentHelper.Models;
using RentHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Helpers
{
    public class OrderQueueHelper
    {
        
        public int PlaceOrderDataBase(Order odr)
        {
            List<OrderExchangeItem> oeItemList = new List<OrderExchangeItem>();
            if (odr.OrderExchangeItems != null) { oeItemList.AddRange(odr.OrderExchangeItems); }

            //return _orderRepository.addOrderQueue(odr, oeItemList);
            return -99;
        }
    }


    public class OrderResult
    {
        public Guid UserId { get; set; }
        public int Result { get; set; }
    }
}
