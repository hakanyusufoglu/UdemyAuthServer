using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.API.Controllers
{

    //POST PUT İŞLEMLERİNDE MUTLAKA CLASS AL
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        //Bu Controller;
        /*
         Yeni bir token alma
         Token silme
          Yeni bir refresh token alma işlemlerini gerçekleştirmektedir.
         */

        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // post => .../api/auth/createtoken 
        //Kullanıcılar için;
        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result = await _authenticationService.CreateTokenAsync(loginDto);

                /*
                 
                 if(result.StatusCode == 200)
                {
                    return ok(result)
                }
                else if(result.status == 404){
                return notfound(result)
                    }
                 
                 */
                //Aşağıdaki kod yukarıda ki if bloğundan bizi kurtarıyor.
            return ActionResultInstance(result);
        }
        [HttpPost]
        //clientlar için;
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)//keşke async olsaydı 
        {
            var result =  _authenticationService.CreateTokenByClient(clientLoginDto);
            return ActionResultInstance(result); //resultin içinde zaten T verisi dolu olduğundan buraya <> değilde () ile doldurduk yani generic vermene gerek yok
        }
        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token);//keşke async olsaydı
            return ActionResultInstance(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }
    }

}
