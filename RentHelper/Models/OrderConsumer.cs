using RentHelper.Helpers;
using RentHelper.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class OrderConsumer
    {
        //需更改為單例
        private static readonly object thisLock = new object();
        private static OrderConsumer instance;

        public static ConcurrentQueue<Order> QueueOrders = new ConcurrentQueue<Order>();
        public static List<OrderResult> OrderResults = new List<OrderResult>();

        private OrderConsumer() {
        
        }

        public static OrderConsumer GetInstance() 
        {
            if (null == instance)
            {
                // 避免多執行緒可能會產生兩個以上的實例，所以 lock
                lock (thisLock)
                {
                    // lock 後，再判斷一次目前有無實例
                    if (null == instance)
                    {
                        instance = new OrderConsumer();
                    }
                }
            }
            return instance;
        }

        public void StartOrderTask()
        {
            while (true)
            {
                //如果沒有訂單任務就休息0.5秒鐘
                if (!QueueOrders.TryDequeue(out Order odr))
                {
                    Thread.Sleep(500);
                    continue;
                }
                //執行真實的業務邏輯（如插入資料庫）
                int rel = new OrderQueueHelper().PlaceOrderDataBase(odr);
                //將執行結果寫入結果集合
                OrderResults.Add(new OrderResult() { Result = rel, UserId = (Guid)odr.Lender });
            }
        }
    }
}
