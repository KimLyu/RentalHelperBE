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
    public class NotesController : ControllerBase
    {
        private readonly JwtHelpers _jwt;
        private INoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public NotesController(
            JwtHelpers jwt,
            INoteRepository noteRepository,
            IMapper mapper
            )
        {
            _jwt = jwt;
            _noteRepository = noteRepository;
            _mapper = mapper;
        }


        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> addNote(
            [FromHeader(Name = "Authorization")] string tokenString,
            [FromBody] Note n
            ) 
        {
            var uid = getUsrIdFromToken(tokenString);
            n.SenderId = uid;
            n.createTime = DateTime.Now;
            if (n.Message == null || n.Message == "") { return BadRequest("無訊息內容"); }
            var errorCode = await _noteRepository.addNote(uid,n);
            switch (errorCode)
            {
                case -5:
                    return BadRequest("無效使用者");
                case -4:
                    return BadRequest("無效訂單ID");
                case -3:
                    return BadRequest("非該訂單關聯者");
                case -2:
                    return BadRequest("重複發送");
                case -1:
                    return BadRequest("建立通知失敗");
                case 0:
                     _noteRepository.firebasePushNotifaction(n);
                    return Ok(_mapper.Map<NoteDto>(n));
                default:
                    return BadRequest("不明異常");
            }
            
        }

        [Authorize]
        [HttpGet("listBy/{OrderId}")]
        public async Task<IActionResult> getList(
            [FromHeader(Name = "Authorization")] string tokenString,
            String OrderId
            )
        {
            var uid = getUsrIdFromToken(tokenString);

            Guid oid = new Guid();
            if (!Guid.TryParse(OrderId, out oid))
            {
                return BadRequest("產品ID錯誤");
            }

            var nlist = await _noteRepository.getList(uid,oid);
            if (nlist == null || nlist.Count() <= 0) { return NotFound("無相關資料"); }

            return Ok(_mapper.Map<List<NoteDto>>(nlist));
        }

        [Authorize]
        [HttpDelete("delete/{noteId}")]
        public async Task<IActionResult> deleteNote(
            [FromHeader(Name = "Authorization")] string tokenString,
            String noteId
            )
        {
            var uid = getUsrIdFromToken(tokenString);

            Guid nid = new Guid();
            if (!Guid.TryParse(noteId, out nid))
            {
                return BadRequest("訊息ID錯誤");
            }

            var errorCode = await _noteRepository.delNote(uid, nid);
            switch (errorCode) {
                case -4:
                    return BadRequest("無效使用者Id");
                case -3:
                    return BadRequest("無效訊息Id");
                case -2:
                    return BadRequest("無操作權限");
                case -1:
                    return BadRequest("系統異常，無法刪除訊息");
                case 0:
                    return Ok("已刪除該訊息");
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
