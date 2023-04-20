using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;

namespace UdemyAuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        //kullanıcıya token dağıtımı yapacak yer
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;
        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            //Her işlemden sonra ifle kontrol ediyoruz buna savunmacı yaklaşımı adı verilmektedir.


            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));//logindto hakkında hata fırlat
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user == null) //email yok
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400,true);//email veya şifra yanlış dedik ki kötü kullanıcı bunu anlamasın client taraflı hata olduğunda 400 diyoruz
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))//paswordu kontrol ediyoruz
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync(); //veritabanında daha önce refresh token var mı?
            if (userRefreshToken == null) //yoksa
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpriration
                });
            }
            else //varsa
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpriration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200); //tokenla birlite dönebilirim.
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            //gelen client var mı yok mu
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if(client == null)
            {
                return  Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404,false);
            }
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if(existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found", 404, true);
            }
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if(user == null)
            {
                return Response<TokenDto>.Fail("User Id not found", 404, true);
            }
            var tokenDto = _tokenService.CreateToken(user);
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpriration;
            //güncelleme
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
                    
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);
            }
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }

}
