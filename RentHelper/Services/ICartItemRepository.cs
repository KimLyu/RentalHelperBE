using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public interface ICartItemRepository
    {
        Task<int> addCartItem(CartItem cItem);
        Task<IEnumerable<CartItem>> getCartItems(Guid uid);
        Task<IEnumerable<Product>> getCartProducts(Guid uid);
        Task<int> delCartItem(Guid uid, Guid pid);
    }
}
