using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Core.Services
{
    //sadece iç yapıda kullanıcağız yani bir api tarafında kullanılmayacak (kullanıcı tarafında kullanmayacağız.)
   public interface ITokenService
    {
        //response dönmedik authantication interfaceinde döncez bunda değil
        TokenDto CreateToken(UserApp userApp);
        ClientTokenDto CreateTokenByClient(Client client); //client entity ve dto değil
    }
}
