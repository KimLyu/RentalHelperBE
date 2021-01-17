using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace RentHelper.Controllers
{
    [Route("[controller]")] //路由聲明
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        public IActionResult test([FromBody] JsonElement requestBody)
        {//動作函數 
            return Ok(requestBody);
        }
    }
}
