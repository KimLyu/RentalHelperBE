using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public interface IUserRepository
    {
        bool isUserExist(string email);//查詢帳號是否存在
        Task<Guid?> checkUser(string email, string pwd, string dTK);//登入後 取得userID

        Task<int> sendEmailcheck(string email);

        Task<int> addUser(User user, string verifycodes);//新增使用者 回傳errorcode

        Task<User> getUserBase(Guid userId);//取得使用者基本資料)
        Task<User> getUser(Guid userId);//取得使用者資料

        Task<int> userUpdate(User user);//更新使用者資訊

        Task<int> userPwdByVerify(string Email,String VerifyCode,String newPwd);
        Task<int> userPwdChange(Guid usrID, string oldPwd, string newPwd);

        Task<int> userDelete(Guid userId);

        //新增願望池清單部分
        Task<int> addWish(Guid uid,WishItem wishItem);
        Task<IEnumerable<WishItem>> getWishList(Guid uid);
        Task<IEnumerable<WishItem>> getWishListOnShelf(Guid uid);
        Task<int> delWishById(Guid uid, Guid wishItemId);
    }
}
