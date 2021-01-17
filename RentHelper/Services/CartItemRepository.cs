using Microsoft.EntityFrameworkCore;
using RentHelper.Database;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public class CartItemRepository: ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public Task<int> addCartItem(CartItem cItem)
        {
            return Task<int>.Run(()=> {
                if (!_context.users.Any(u => u.Id == cItem.UserId)) { return -4; }//無此使用者
                if (!_context.products.Any(p => p.Id == cItem.ProductId)) { return -3; }//無此產品
                if (_context.cartitems.Any(ci => ci.UserId == cItem.UserId && ci.ProductId == cItem.ProductId)) { return -2; }

                _context.cartitems.Add(cItem);
                save();
                var ci = _context.cartitems.FirstOrDefault(ci => ci == cItem);//回存
                if (ci != null) { cItem.Id = ci.Id; }
                return (ci != null) ? 0 : -1;
            });
        }
        public Task<IEnumerable<CartItem>> getCartItems(Guid uid)
        {
            return Task.FromResult < IEnumerable < CartItem >> (_context.cartitems
                .Where(ci => ci.UserId == uid));
        }

        public Task<IEnumerable<Product>> getCartProducts(Guid uid)
        {
            var plist = _context.cartitems
                .Where(ci => ci.UserId == uid)
                .Join(
                    _context.products
                        .Include(p => p.Pics),
                    ci => ci.ProductId,
                    p => p.Id,
                    (ci,p) => p);//交集兩張表，輸出符合的product資料

            return Task.FromResult<IEnumerable<Product>>(plist);
        }

        public Task<int> delCartItem(Guid uid, Guid pid)
        {
            return Task<int>.Run(()=> {
                var citem = _context.cartitems.FirstOrDefault(ci => ci.UserId == uid && ci.ProductId == pid);
                if (citem == null) { return -2; }
                _context.cartitems.Remove(citem);
                save();
                return _context.cartitems.Any(ci => ci.UserId == uid && ci.ProductId == pid) ? -1 : 0;
            });
        }

        public int save()
        {
            return _context.SaveChanges(); //需要執行 資料庫更新
        }

        
    }
}
