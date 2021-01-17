using RentHelper.Dtos;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public interface IOrderRepository
    {
        bool isOrderExist(Guid OrderId);

        public int addOrderQueue(Order order, List<OrderExchangeItem> oeItems);
        Task<int> addOrder(Order order,List<OrderExchangeItem> oeItems);

        Task<IEnumerable<Order>> getMyOrders(Guid uid);

        Task<IEnumerable<Order>> buyList(Guid lenderId, String status);

        Task<IEnumerable<Order>> sellList(Guid ownerId, String status);

        Task<Order> getOrderById(Guid ownerId, Guid OrderId);

        //訂單狀態修改
        Task<int> orderStatusChange(Guid userId, Guid orderId, string nowStatus, OrderStatusDto OrderStatusDto);
        void firebasePushNotifaction(Guid uid, Guid oid, OrderStatusDto oStatusDto);
        Task<int> OrderDelete(Guid uid, Guid OrderId);

    }
}
