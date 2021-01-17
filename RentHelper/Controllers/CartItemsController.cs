using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentHelper.Dtos;
using RentHelper.Helpers;
using RentHelper.Models;
using RentHelper.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RentHelper.Controllers
{
    [Route("[controller]")] //路由聲明
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly JwtHelpers _jwt;
        private ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;

        public CartItemsController(
            JwtHelpers jwt,
            ICartItemRepository cartItemRepository,
            IMapper mapper
            )
        {
            _jwt = jwt;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> addCartItem(
            [FromBody] JsonElement  requestBody,
            [FromHeader(Name = "Authorization")] string tokenString
            )
        {
            Guid uid = getUsrIdFromToken(tokenString);
            JsonElement pidJe = new JsonElement();
            String pidStr = requestBody.TryGetProperty("ProductId", out pidJe) ? pidJe.GetString() : "";
            Guid pid = new Guid();
            if(!Guid.TryParse(pidStr,out pid))
            {
                return BadRequest("產品ID格式錯誤");
            }

            var cItem = new CartItem()
            {
                ProductId = pid,
                UserId = uid
            };

            var errorCode = await _cartItemRepository.addCartItem(cItem);

            switch (errorCode) {
                case -2:
                    return BadRequest("重複添加!");
                case -1:
                    return BadRequest("購物車添加商品失敗!");
                case 0:
                    return Ok(cItem.Id.ToString());
                default:
                    return BadRequest("不明異常!");
            }
        }

        [Authorize]
        [HttpGet("productList")]
        public async Task<IActionResult> getCartProduct([FromHeader(Name = "Authorization")] string tokenString)
        {
            Guid uid = getUsrIdFromToken(tokenString);
            var plist = await _cartItemRepository.getCartProducts(uid);
            if(plist == null || plist.Count() <= 0) { return NotFound("無相關清單"); }
            return Ok(_mapper.Map<List<ProductDto>>(plist));
        }

        [Authorize]
        [HttpGet("cartItemList")]
        public async Task<IActionResult> getCartItems([FromHeader(Name = "Authorization")] string tokenString)
        {
            Guid uid = getUsrIdFromToken(tokenString);
            var cilist = await _cartItemRepository.getCartItems(uid);
            if (cilist == null || cilist.Count() <= 0) { return NotFound("無相關清單"); }
            return Ok(_mapper.Map<List<CartItemDto>>(cilist));
        }

        [Authorize]
        [HttpDelete("{ProductId}")]
        public async Task<IActionResult> DeleteCartItem(
            [FromHeader(Name = "Authorization")] string tokenString,
            string ProductId
            )
        {
            Guid uid = getUsrIdFromToken(tokenString);
            Guid pid = new Guid();
            if (!Guid.TryParse(ProductId, out pid))
            {
                return BadRequest("產品ID格式錯誤");
            }
            var errorCode = await _cartItemRepository.delCartItem(uid,pid);

            switch (errorCode) {
                case -2:
                    return BadRequest("該產品不在購物車內");
                case -1:
                    return BadRequest("購物車物品移出失敗");
                case 0:
                    return Ok("已將產品移出購物車");
                default:
                    return BadRequest("不明異常");
            }
            
        }


        private Guid getUsrIdFromToken(string tokenString)
        {
            var jwtEncodedString = tokenString.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            var tokenusrid = token.Claims.First(c => c.Type == "userid").Value;
            return Guid.Parse(tokenusrid);
        }
    }
}
