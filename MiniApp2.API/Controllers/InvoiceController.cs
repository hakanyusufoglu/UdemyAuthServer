using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp2.API.Controllers
{
    //ÜYELİK SİSTEMİYLE İLGİLİ DATA ALINACAK VS. (senaryo fatura bilgileri)
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInVoice()
        {
            var userName = HttpContext.User.Identity.Name; //tokenin payloadundan geliyor
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            //veri tabanında userId veya username alanları üzerinden gerekli dataları çek
            return Ok($"Invoice Operations => UserName:{userName} - UserId:{userIdClaim.Value}");
        }
    }
}
