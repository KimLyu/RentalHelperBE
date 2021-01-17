using Microsoft.AspNetCore.Mvc;
using RentHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RentHelper.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using RentHelper.Models;
using System.Globalization;

namespace RentHelper.Controllers
{

    [Authorize]
    [Route("[controller]")] //路由聲明
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderRepository orderRepository, 
            IMapper mapper
            )
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            //Task.Run(OrderConsumer.GetInstance().StartOrderTask(_orderRepository));
        }

        [HttpPost("addQueue")]
        public async Task<IActionResult> addOrderQueue(
            [FromHeader(Name = "Authorization")] string tokenString,
            [FromBody] Order newOrder)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);

            if (newOrder.ProductId == null || newOrder.TradeQuantity == 0 || newOrder.TradeMethod < 0 || newOrder.TradeMethod > 2)
            {
                return NotFound("輸入資料錯誤，訂單新增失敗");
            }

            //只取需要建立的部分，多餘的部分排除
            Order odr = new Order
            {
                //交易方式
                TradeMethod = newOrder.TradeMethod, //訂單使用的購買方式
                TradeQuantity = newOrder.TradeQuantity, //添加 交易的數量
                                                        //資訊及自動補充
                OrderTime = DateTime.Now,
                status = OrderStatus.CREATE,
                Lender = uid,   //租借或購買人 = 自己
                ProductId = newOrder.ProductId //添加產品資訊
            };

            OrderConsumer.QueueOrders.Enqueue(odr);//未處理的odr加入對列當中

            var errorCode = await getOrderResult(odr);

            return errorCode switch
            {
                -9 => BadRequest("回應超時"),
                -8 => BadRequest("不允許的交易模式"),
                -7 => BadRequest("願望清單項目異常"),
                -6 => BadRequest("交易數量超過需求數量"),
                -5 => BadRequest("交換項目數量不符"),
                -4 => BadRequest("交易數量異常"),
                -3 => BadRequest("無此產品ID"),
                -2 => BadRequest("無此使用者ID"),
                -1 => BadRequest("訂單建立失敗"),
                0 => Ok(odr.Id.ToString()),
                _ => BadRequest("不明異常"),
            };
        }
        private Task<int> getOrderResult(Order odr) {
            var time = DateTime.Now;
            var errorCode = 0;
            while (true)
            {//確認是否處理完成
                var rel = OrderConsumer.OrderResults.FirstOrDefault(OdrRel => OdrRel.UserId == odr.Lender);
                if (rel != null)
                {
                    errorCode = rel.Result;
                    break;
                }
                //if (DateTime.Now.Subtract(time).Seconds < 60)
                //{
                //    //超過60秒無回應
                //    errorCode = -9;
                //    break;
                //}
            }
            return Task.FromResult<int>(errorCode);
        }

        [HttpPost("add")]
        public async Task<IActionResult> addOrder(
            [FromHeader(Name = "Authorization")] string tokenString,
            [FromBody] Order newOrder)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);

            if (newOrder.ProductId == null || newOrder.TradeQuantity == 0 || newOrder.TradeMethod < 0 || newOrder.TradeMethod > 2)
            {
                return NotFound("輸入資料錯誤，訂單新增失敗");
            }

            //只取需要建立的部分，多餘的部分排除
            Order odr = new Order
            {
                //交易方式
                TradeMethod = newOrder.TradeMethod, //訂單使用的購買方式
                TradeQuantity = newOrder.TradeQuantity, //添加 交易的數量
                                                        //資訊及自動補充
                OrderTime = DateTime.Now,
                status = OrderStatus.CREATE,
                Lender = uid,   //租借或購買人 = 自己
                ProductId = newOrder.ProductId //添加產品資訊
            };

            List<OrderExchangeItem> oeItemList = new List<OrderExchangeItem>();
            if (newOrder.OrderExchangeItems != null) { oeItemList.AddRange(newOrder.OrderExchangeItems); }

            var errorCode = await _orderRepository.addOrder(odr, oeItemList);

            return errorCode switch
            {
                -8 => BadRequest("不允許的交易模式"),
                -7 => BadRequest("願望清單項目異常"),
                -6 => BadRequest("交易數量超過需求數量"),
                -5 => BadRequest("交換項目數量不符"),
                -4 => BadRequest("交易數量異常"),
                -3 => BadRequest("無此產品ID"),
                -2 => BadRequest("無此使用者ID"),
                -1 => BadRequest("訂單建立失敗"),
                0 => Ok(odr.Id.ToString()),
                _ => BadRequest("不明異常"),
            };
        }

        [HttpGet("Mylist")]
        public async Task<IActionResult> getMyOrderList([FromHeader(Name = "Authorization")] string tokenString)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);
            var olist = await _orderRepository.getMyOrders(uid);

            if (olist == null || olist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }

            return Ok(_mapper.Map<List<OrderDto>>(olist));
        }

        [HttpGet("Mylist/buyer/{status}")]
        public async Task<IActionResult> getMyOrderBuyList(
            [FromHeader(Name = "Authorization")] string tokenString,
            string status)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);
            var olist =await  _orderRepository.buyList(uid, status);

            if (olist == null || olist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<OrderDto>>(olist));
        }

        [HttpGet("Mylist/seller/{status}")]
        public async Task<IActionResult> getMyOrderSellList(
            [FromHeader(Name = "Authorization")] string tokenString,
            string status)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);
            var olist = await _orderRepository.sellList(uid, status);

            if (olist == null || olist.Count() <= 0)
            {
                return NotFound("無相關項目");
            }
            return Ok(_mapper.Map<List<OrderDto>>(olist));
        }


        [HttpGet("{Orderid}")]
        public async Task<IActionResult> getOrder(
            [FromHeader(Name = "Authorization")] string tokenString,
            string Orderid)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);

            if (!Guid.TryParse(Orderid, out Guid oid)) { return BadRequest("訂單格式錯誤"); }
            var order = await _orderRepository.getOrderById(uid,oid);
            if(order == null) { return NotFound("查無相關訂單"); }

            return Ok( _mapper.Map<OrderDto>(order));
        }

        
        [HttpDelete("{Orderid}")]
        public async Task<IActionResult> orderDelete(
            [FromHeader(Name = "Authorization")] string tokenString,
            string Orderid)
        {//動作函數 
            Guid uid = getUsrIdFromToken(tokenString);

            if (!Guid.TryParse(Orderid, out Guid oid)) { return BadRequest("訂單格式錯誤"); }

            var errorCode = await _orderRepository.OrderDelete(uid, oid);

            return errorCode switch
            {
                -5 => BadRequest("訂單狀態 不允許刪除"),
                -4 => BadRequest("無刪除權限"),
                -3 => BadRequest("訂單ID已失效"),
                -2 => BadRequest("使用者ID失效"),
                -1 => BadRequest("刪除失敗"),
                0 => Ok("已刪除訂單"),
                _ => BadRequest("不明異常"),
            };
        }

        [HttpPatch("status/{Orderid}/{nowStatus}")]
        public async Task<IActionResult> statusUpdate(
            [FromHeader(Name = "Authorization")] string tokenString,
            string Orderid,string nowStatus)
        {//動作函數 
            if (!Guid.TryParse(Orderid, out Guid oid)) { return BadRequest("訂單格式錯誤"); }
            var uid = getUsrIdFromToken(tokenString);

            var tmpOdrStatus = new OrderStatusDto();
            
            var errorCode = await _orderRepository.orderStatusChange(uid, oid, nowStatus, tmpOdrStatus);
            
            switch (errorCode) 
            {
                case -6:
                    return BadRequest("無效訂單ID");
                case -5:
                    return BadRequest("無權限使用者ID");
                case -4:
                    return BadRequest("狀態設定錯誤");
                case -3:
                    return BadRequest("非目前訂單狀態");
                case -2:
                    return BadRequest("操作錯誤");
                case -1:
                    return BadRequest("更新狀態失敗");
                case 0:
                    _orderRepository.firebasePushNotifaction(uid, oid, tmpOdrStatus);
                    return Ok(tmpOdrStatus);
                default:
                    return BadRequest("不明異常");

            }

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
