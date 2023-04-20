using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.Core.Services
{
   //üye kaydetme işlemini gerçekleştirecek. Veritabanında işlem yapaıcağız ama repository oluşturmadım. Bunun sebebi Identity kütüphanesiyle birlikte ordan hazır metotlar gelicek ve bu yüzden ayrı bir repository oluşturmama gerek yok.
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

        //username göre veritabanından kullanıcı bulalım
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
    }
}
