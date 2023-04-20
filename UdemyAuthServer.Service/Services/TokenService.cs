using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.Service.Services
{
    public class TokenService : ITokenService 
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }
        private string CreateRefreshToken()//random string değer üretir ve unique.
        {
            var numberByte = new Byte[32]; //32 bytelik random string üreteceğiz
            using var rnd = RandomNumberGenerator.Create(); //using{} te olabilir
            rnd.GetBytes(numberByte); //byte alırken yukarıdaki numberbyte aktar diyorum
            return Convert.ToBase64String(numberByte);
        }
        //tokenda ki payloadları eklicez. Bu metot login barındıran uygulama için...
        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences) //kullanıcı bilgileri ve bu token hangi apilere karşılık geleceğini alıyorum.
        {

            //tokenda kullanıcı ile ilgili gerekli bilgileri tutuyorum
            var userList = new List<Claim> { new Claim(
                   //type = key, 
                   ClaimTypes.NameIdentifier, //id yani bir kimlik
                   userApp.Id   //kullanıcının idsini tut 
                ),
            new Claim(
                JwtRegisteredClaimNames.Email,  //yada ClaimTypes.Email 
                userApp.Email
                ),
            new Claim (ClaimTypes.Name,userApp.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()) //her tokena random kimlik veriyoruz zorunlu değil ancak BestPractice için
            
            };
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());//kimin için bu token?
            return claims;

        }
        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
          
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);//imzayı buradan oluşturuyoruz

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer:_tokenOption.Issuer, //bu tokeni yayınlayan kim?
                expires:accessTokenExpiration, //ne kadar süre çalışacak bu token,
                notBefore: DateTime.Now,//benim verdiğim saatten itibaren çalışsın 
                claims:GetClaims(userApp,_tokenOption.Audience),
                signingCredentials: signingCredentials
                );


            var handler = new JwtSecurityTokenHandler(); //burası bir token oluşturacak
            var token = handler.WriteToken(jwtSecurityToken); //token oluşturduk.
            //tokeni tokendtoya çevirdil
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpriration = accessTokenExpiration,
                RefreshTokenExpriration = refreshTokenExpiration
            };
            return tokenDto;

        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);//imzayı buradan oluşturuyoruz

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer, //bu tokeni yayınlayan kim?
                expires: accessTokenExpiration, //ne kadar süre çalışacak bu token,
                notBefore: DateTime.Now,//benim verdiğim saatten itibaren çalışsın 
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );


            var handler = new JwtSecurityTokenHandler(); //burası bir token oluşturacak
            var token = handler.WriteToken(jwtSecurityToken); //token oluşturduk.
            //tokeni tokendtoya çevirdil
            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpriration = accessTokenExpiration
            };
            return clientTokenDto;
        }
    }
}
