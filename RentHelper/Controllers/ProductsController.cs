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
using RentHelper.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Drawing;

namespace RentHelper.Controllers
{
    [Route("[controller]")] //路由聲明
    [ApiController]
    public class ProductsController : ControllerBase
    {
       
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(
            IProductRepository productRepository,
            IMapper mapper
            )
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> addProduct(
            [FromBody] Product p,
            [FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 

            p.UserId = getUsrIdFromToken(tokenString);

            List<Picture> pics = new List<Picture>();
            if (p.Pics != null && p.Pics.Count() > 0)
            {
                pics.AddRange(p.Pics);
                p.Pics.Clear();
            }//清除圖片資訊，拆開建立
            

            var errorcode = await _productRepository.addProduct(p, pics);

            return errorcode switch
            {
                -3 => BadRequest("交換項目有效數量異常，請確認交換設定"),
                -2 => BadRequest("交換方式資料異常 errorcode: " + errorcode),
                -1 => BadRequest("產品創建失敗 errorcode: " + errorcode),
                0 => Ok(p.Id.ToString()),
                _ => BadRequest("不明異常"),
            };
        }

        [HttpGet("listBy/{type}/{type1}/{type2}/{index}/{length}")]
        public async Task<IActionResult> ListBy(string type, string type1,string type2,string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listBy(type,type1,type2 ,index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }

            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        //待刪除
        [HttpGet("listByType/{type}/{index}/{length}")]
        public async Task<IActionResult> pageListByType(string type, string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listByBrand(type, index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }

            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        [HttpGet("listByTypeNType2/{type}/{type2}/{index}/{length}")]
        public async Task<IActionResult> pageListByTypeNTType2(string type, string type2, string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listByBrandNType2(type, type2, index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }

            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        [HttpGet("listByType1/{type1}/{index}/{length}")]
        public async Task<IActionResult> pageListByType1(string type1, string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listByType1(type1, index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        [HttpGet("listByType2/{type1}/{type2}/{index}/{length}")]
        public async Task<IActionResult> pageListByType2(string type1, string type2, string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listByType2(type1, type2, index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }
        //待刪除


        [HttpGet("listByKeyWord/{type1}/{type2}/{keyword}/{index}/{length}")]
        public async Task<IActionResult> pageListBySearch(string type1, string type2, string keyword, string index, string length)
        {//動作函數 
            var pagelist = await _productRepository.listByKeyword(type1, type2, keyword, index, length);
            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        [HttpGet("listBySeller/{sellerid}/{index}/{length}")]
        public async Task<IActionResult> listBySeller(string sellerid, string index, string length)
        {//動作函數 

            if (!Guid.TryParse(sellerid, out Guid sellerId))
            {
                return BadRequest("使用者ID格式錯誤");
            }

            var pagelist = await _productRepository.listBySeller(sellerId, index, length);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }

            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        [HttpGet("ById/{id}")]
        public async Task<IActionResult> productGetById(string id)
        {//動作函數 
            if (!Guid.TryParse(id, out Guid pid))
            {
                return BadRequest("產品ID錯誤");
            }
            var p = await _productRepository.getProduct(pid);
            if (p == null)
            {
                return NotFound("無此ID產品");
            }
            return Ok(_mapper.Map<ProductDto>(p));
        }

        [Authorize]
        [HttpGet("ownItem")]
        public async Task<IActionResult> ownItemList([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var pagelist = await _productRepository.myOwnList(uid);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }
        //上架中
        [Authorize]
        [HttpGet("ownItemOnShelf")]
        public async Task<IActionResult> myOwnListOnShelf(
            [FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var pagelist = await _productRepository.myOwnListOnShelf(uid);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        //未上架
        [Authorize]
        [HttpGet("ownItemNotOnShelf")]
        public async Task<IActionResult> myOwnListNotOnShelf(
            [FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var pagelist = await _productRepository.myOwnListNotOnShelf(uid);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }

        //根據訂單狀態取得buyer身分列表
        [Authorize]
        [HttpGet("ownItemByStatus/buyer/{status}")]
        public async Task<IActionResult> myOwnListByOrderStatusBuyer(
            [FromHeader(Name = "Authorization")] string tokenString,
            string status)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var pagelist = await _productRepository.myOwnListByOrderStatusBuyer(uid, status);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }
        //根據訂單狀態取得seller身分列表
        [Authorize]
        [HttpGet("ownItemByStatus/seller/{status}")]
        public async Task<IActionResult> myOwnListByOrderStatusSeller(
            [FromHeader(Name = "Authorization")] string tokenString,
            string status)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var pagelist = await _productRepository.myOwnListByOrderStatusSeller(uid, status);

            if (pagelist == null || pagelist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<ProductDto>>(pagelist));
        }


        [Authorize]
        [HttpGet("ownItem/{productid}")]//查詢
        public async Task<IActionResult> ownItemById([FromHeader(Name = "Authorization")] string tokenString, string productid)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            if (!Guid.TryParse(productid, out Guid pid))
            {
                return BadRequest("產品ID格式錯誤");
            }

            var p = await _productRepository.getMyProduct(uid, pid);

            if (p == null)
            {
                return NotFound("查無此產品");
            }

            return Ok(_mapper.Map<ProductDto>(p));
        }

        [Authorize]
        [HttpPatch("modify")]//修改用
        public async Task<IActionResult> ownItemUpdateById([FromHeader(Name = "Authorization")] string tokenString, [FromBody] Product p)
        {//動作函數 

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯  

            var errorcode = await _productRepository.productUpdate(p, uid);

            return errorcode switch
            {
                -2 => NotFound("查無此ID," + errorcode),
                //case -1:
                //    return BadRequest("更新失敗," + errorcode);
                0 => Ok("ownItemUpdateById success!!," + errorcode),
                _ => BadRequest("不明異常," + errorcode),
            };
        }

        [Authorize]
        [HttpDelete("ownItem/{productid}")]
        public async Task<IActionResult> ownItemDeleteById([FromHeader(Name = "Authorization")] string tokenString, string productid)
        {//動作函數 

            if (!Guid.TryParse(productid, out Guid pid))
            {
                return BadRequest("產品ID格式錯誤");
            }

            Guid uid = getUsrIdFromToken(tokenString);//token uid不會錯 

            var errorCode = await _productRepository.productDelete(pid, uid);
            return errorCode switch
            {
                -2 => BadRequest("無此產品id"),
                -1 => BadRequest("刪除失敗"),
                0 => Ok("產品刪除成功"),
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

        [HttpGet("PictureClear")]
        public IActionResult PictureClear()
        {//動作函數 
            _productRepository.PicClr();
            return Ok();
        }
    }
}
