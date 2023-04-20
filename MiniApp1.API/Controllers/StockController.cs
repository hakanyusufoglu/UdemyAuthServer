using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp1.API.Controllers
{
    //ÜYELİK SİSTEMİYLE İLGİLİ DATA ALINACAK VS.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name; //tokenin payloadundan geliyor
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            //veri tabanında userId veya username alanları üzerinden gerekli dataları çek
            return Ok($"Stock Operations => UserName:{userName} - UserId:{userIdClaim.Value}");
        }
    }
}
