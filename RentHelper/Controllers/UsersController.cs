using Microsoft.AspNetCore.Mvc;
using RentHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RentHelper.Dtos;
using RentHelper.Models;
using System.Text.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using RentHelper.Helpers;

namespace RentHelper.Controllers
{
    
    [Route("[controller]")] //路由聲明
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly JwtHelpers _jwt;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(
            JwtHelpers jwt,
            IUserRepository userRepository,
            IMapper mapper
            )
        {
            _jwt = jwt;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //[AllowAnonymous]//允許無token操作
        [HttpPost("login")]
        public async Task<IActionResult> userLogin([FromBody] JsonElement requestBody)
        {//動作函數 


            string acc = requestBody.TryGetProperty("Email", out JsonElement accJe) ? accJe.GetString() : "";
            string pwd = requestBody.TryGetProperty("Password", out JsonElement pwdJe) ? pwdJe.GetString() : "";
            string dTK = requestBody.TryGetProperty("DeviceToken", out JsonElement dTKJe) ? dTKJe.GetString() : "";
            if (acc == "" || pwd == "" )
            {
                return NotFound("資料錯誤");
            }

            var usrid =  await _userRepository.checkUser(acc, pwd,dTK);
            if(usrid == null)
            {
                return NotFound("資料錯誤或無此使用者");
            }
            string token = _jwt.GenerateToken(usrid.ToString());
            return  Ok(token);
        }

        //[AllowAnonymous]//允許無token操作
        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] JsonElement reqBody)
        {//動作函數 

            User usr = new User()
            {
                Email = reqBody.TryGetProperty("Email", out JsonElement accJe) ? accJe.GetString() : null,
                Password = reqBody.TryGetProperty("Password", out JsonElement pwdJe) ? pwdJe.GetString() : null,
                Name = reqBody.TryGetProperty("Name", out JsonElement nameJe) ? nameJe.GetString() : null,
                NickName = reqBody.TryGetProperty("NickName", out JsonElement nickJe) ? nickJe.GetString() : null,
                Phone = reqBody.TryGetProperty("Phone", out JsonElement phoneJe) ? phoneJe.GetString() : null,
                Address = reqBody.TryGetProperty("Address", out JsonElement addressJe) ? addressJe.GetString() : null
            };

            if (usr.Email == null || usr.Password == null || usr.Name == null || usr.Phone == null) {
                return BadRequest("使用者資料不齊全，註冊失敗");
            }

            string verityCode = reqBody.TryGetProperty("VerityCode", out JsonElement vCodeJe) ? vCodeJe.GetString() : "";
            var tmpPwd = usr.Password;
            var errorcode = await _userRepository.addUser(usr, verityCode);

            switch (errorcode) {
                case -4:
                    return BadRequest("未進行信件確認 或 確認失敗!");
                case -3:
                    return BadRequest("信件確認超時!");
                case -2:
                    return BadRequest("該帳號已存在!");
                case -1:
                    return BadRequest("帳號創建失敗!");
                case 0:
                    var uid = _userRepository.checkUser(usr.Email, tmpPwd,"");
                    if (uid == null) { return BadRequest("uid=null"); }
                    return Ok(uid.ToString());
                default:
                    return BadRequest("不明異常!");
            }
        }

        [HttpGet("baseInfo/{id}")]
        public async Task<IActionResult> baseInfoGet(string id)
        {//動作函數 
            if (!Guid.TryParse(id, out Guid uid))
            {
                return BadRequest("賣家ID格式錯誤");
            }
            User usrInfo = await _userRepository.getUserBase(uid);
            if (usrInfo == null)
            {
                return NotFound("無此ID使用者");
            }
            UserDto userDto = _mapper.Map<UserDto>(usrInfo);
            return Ok(userDto);
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> infoGet([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 

            User usrInfo = await _userRepository.getUser(getUsrIdFromToken(tokenString));
            if (usrInfo == null) {
                return NotFound("無此ID使用者");
            }
            UserDto userDto = _mapper.Map<UserDto>(usrInfo);//圖片也自動隱含對應 願望清單也自動隱含對應
            return Ok(userDto);
        }

        [Authorize]
        [HttpPatch("info")]
        public async Task<IActionResult> infoUpdate(
            [FromHeader(Name = "Authorization")] string tokenString, 
            [FromBody] JsonElement requestBody)
        {//動作函數 

            var usr = await _userRepository.getUser(getUsrIdFromToken(tokenString));


            if (requestBody.TryGetProperty("address", out JsonElement addressJe))
            { usr.Address = addressJe.GetString(); }
            if (requestBody.TryGetProperty("name", out JsonElement nameJe))
            { usr.Name = nameJe.GetString(); }
            if (requestBody.TryGetProperty("nickName", out JsonElement nickJe))
            { usr.NickName = nickJe.GetString(); }
            if (requestBody.TryGetProperty("phone", out JsonElement phoneJe))
            { usr.Phone = phoneJe.GetString(); }

            var errorcode =await _userRepository.userUpdate(usr);

            return errorcode switch
            {
                -2 => NotFound("查無此ID," + errorcode),
                //case -1:
                //    return BadRequest("更新失敗," + errorcode);
                0 => Ok("infoUpdate success!!," + errorcode),
                _ => BadRequest("不明異常," + errorcode),
            };
        }

        [HttpPost("security/check")]
        public async Task<IActionResult> emailCheck([FromBody] JsonElement requestBody)
        {//動作函數
            string email = requestBody.TryGetProperty("Email", out JsonElement emailJe) ? emailJe.GetString() : "";
            if (email == "")
            {
                return BadRequest("無法寄信");
            }
            var errorCode = await _userRepository.sendEmailcheck(email);
            return errorCode switch
            {
                -2 => BadRequest("非Email格式"),
                -1 => BadRequest("產生認證碼異常"),
                0 => Ok("已寄出確認信件到:" + email),
                _ => BadRequest("不明異常"),
            };
        }

        [HttpPost("security/vp")]
        public async Task<IActionResult> forgetPwdVerify([FromBody] JsonElement reqBody)
        {//動作函數 
            var email = reqBody.TryGetProperty("Email", out JsonElement emailJe) ? emailJe.GetString() : null;
            var vCode = reqBody.TryGetProperty("VerityCode", out JsonElement vCodeJe) ? vCodeJe.GetString() : null;
            var newPwd = reqBody.TryGetProperty("NewPassword", out JsonElement newPwdJe) ? newPwdJe.GetString() : null;
            if (email == null || vCode == null || newPwd == null) { return BadRequest("資料不齊全，請重新操作"); }

            var errorCode = await _userRepository.userPwdByVerify(email, vCode, newPwd);
            return errorCode switch
            {
                -4 => BadRequest("未進行信件確認 或 確認失敗!"),
                -3 => BadRequest("信件確認超時!"),
                -2 => BadRequest("無此使用者"),
                -1 => BadRequest("使用者密碼更新失敗"),
                0 => Ok("使用者密碼更新成功"),
                _ => BadRequest(),
            };
        }

        [Authorize]
        [HttpPost("security/op")]
        public async Task<IActionResult> pwdUpdate(
            [FromHeader(Name = "Authorization")] string tokenString,
            [FromBody] JsonElement reqBody)
        {//動作函數 
            var oldPwd = reqBody.TryGetProperty("OldPassword", out JsonElement oldPwdJe) ? oldPwdJe.GetString() : null;
            var newPwd = reqBody.TryGetProperty("NewPassword", out JsonElement newPwdJe) ? newPwdJe.GetString() : null;
            var errorCode = await _userRepository.userPwdChange(getUsrIdFromToken(tokenString), oldPwd, newPwd);
            return errorCode switch
            {
                -2 => BadRequest("無此ID"),
                -1 => BadRequest("原密碼錯誤"),
                0 => Ok("使用者密碼更新成功!"),
                _ => BadRequest("不明異常"),
            };
        }

        [Authorize]
        [HttpDelete("unRegister")]
        public async Task<IActionResult> unRegister([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 
            var errorCode = await _userRepository.userDelete(getUsrIdFromToken(tokenString));
            return errorCode switch
            {
                -2 => BadRequest("無此ID，操作錯誤!"),
                -1 => BadRequest("刪除失敗!"),
                0 => Ok("已刪除帳號"),
                _ => BadRequest("不明異常"),
            };
        }

        [Authorize]
        [HttpPost("wishlist/new")]
        public async Task<IActionResult> addWish(
            [FromHeader(Name = "Authorization")] string tokenString,
            [FromBody] WishItem wishitem
            )
        {//動作函數 
            var errorCode = await _userRepository.addWish(getUsrIdFromToken(tokenString),wishitem);
            return errorCode switch
            {
                -3 => BadRequest("使用者ID已失效，請重新登入"),
                -2 => BadRequest("輸入資料格式錯誤，請重新輸入"),
                -1 => BadRequest("新增清單失敗"),
                0 => Ok(wishitem.Id.ToString()),
                _ => BadRequest("不明異常"),
            };
        }

        [Authorize]
        [HttpGet("wishlist/All")]//全部的願望清單項目 包含已取得 數量=0
        public async Task<IActionResult> getWishList([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 
            var wlist = await _userRepository.getWishList(getUsrIdFromToken(tokenString));
            if (wlist == null || wlist.Count() <= 0) { return NotFound("查無相關項目"); }
            return Ok(_mapper.Map<List<WishItemDto>>(wlist));
        }

        [Authorize]
        [HttpGet("wishlist/onshelf")]//需要的清單 數量 >0
        public async Task<IActionResult> getWishListOnShelf([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 
            var wlist = await _userRepository.getWishListOnShelf(getUsrIdFromToken(tokenString));
            if (wlist == null || wlist.Count() <= 0) { return NotFound("查無相關項目"); }
            return Ok(_mapper.Map<List<WishItemDto>>(wlist));
        }

        [Authorize]
        [HttpDelete("wishlist/{wishId}")]
        public async Task<IActionResult> delWishById(
            [FromHeader(Name = "Authorization")] string tokenString,
            string wishId
            )
        {//動作函數 
            if (!Guid.TryParse(wishId, out Guid wid))
            {
                return BadRequest("願望清單ID格式錯誤");
            }

            var errorCode = await _userRepository.delWishById(getUsrIdFromToken(tokenString), wid);

            return errorCode switch
            {
                -4 => BadRequest("無效使用者ID，請重新登入"),
                -3 => BadRequest("無效願望項目ID"),
                -2 => BadRequest("非清單持有者，無操作權限"),
                -1 => BadRequest("刪除項目失敗"),
                0 => Ok("刪除項目成功"),
                _ => BadRequest("不明異常"),
            };
        }

        private Guid getUsrIdFromToken(string tokenString)
        {
            var jwtEncodedString = tokenString[7..]; // trim 'Bearer ' from the start since its just a prefix for the token string
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            var tokenusrid = token.Claims.First(c => c.Type == "userid").Value;
            return Guid.Parse(tokenusrid);
        }

    }
}
