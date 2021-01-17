using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public interface IProductRepository
    {
        //bool isProductExist(Guid productId);

        //賣家使用
        Task<int> addProduct(Product product, List<Picture> pics);
        Task<IEnumerable<Product>> myOwnList(Guid userId);
        Task<IEnumerable<Product>> myOwnListNotOnShelf(Guid userId);
        Task<IEnumerable<Product>> myOwnListOnShelf(Guid userId);
        Task<IEnumerable<Product>> myOwnListByOrderStatusBuyer(Guid userId, String status);
        Task<IEnumerable<Product>> myOwnListByOrderStatusSeller(Guid userId, String status);

        Task<Product> getMyProduct(Guid userId, Guid productId);

        ////使用者查詢整合
        Task<IEnumerable<Product>> listBy(string type, string type1, string type2 , string index, string length);

        //使用者查詢(合併)
        Task<IEnumerable<Product>> listByBrand(string type, string index, string length);
        Task<IEnumerable<Product>> listByBrandNType2(string type, string type2, string index, string length);
        Task<IEnumerable<Product>> listByType1(string type1, string index, string length);
        Task<IEnumerable<Product>> listByType2(string type1, string type2, string index, string length);
        //使用者查詢(合併)

        Task<IEnumerable<Product>> listByKeyword(string type1, string type2, string keyword, string index, string length);


        Task<IEnumerable<Product>> listBySeller(Guid SellerId, string index, string length);

        Task<Product> getProduct(Guid productId);

        IEnumerable<Picture> getPicturesByProductId(Guid productId);

        Task<int> productUpdate(Product product, Guid ownerId);

        Task<int> productDelete(Guid productId, Guid ownerId);

        int save();

        void PicClr();//清除多餘圖片用
    }
}
