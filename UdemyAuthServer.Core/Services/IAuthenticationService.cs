using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.Core.Services
{
    //kullanıcıdan email ve password alacağız ve bunu doğrulayacağız. Böyle bir kullanıcı var mı? diye
   public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);//logindto doğruysa geriye token dönecek.
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken); //refresh token ile yeni bir token alacağız.
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken); //eğer bir refresh token varsa onu kaldır. Yani logout yapıyor.
        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto); //bir üyelik sistemi olmadığında jwt süresi dolduğunda direk olarak bu metotla refresh token görevi yapabilirim. Böylece yeni bir token elde etmiş olurum.
    }
}
