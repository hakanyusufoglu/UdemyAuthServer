using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.DTOs
{
    //sadece apide gerekli olacağından buraya tokendto ekledik
   public class TokenDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpriration { get; set; } //tokenin ömrünü tututyoryz zorunlu deği zaten token içinde var
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpriration { get; set; }
    }
}
