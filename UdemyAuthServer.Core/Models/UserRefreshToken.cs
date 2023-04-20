using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Models
{
   public class UserRefreshToken
    {
        public string UserId { get; set; } //bu refresh token kime ait olacak
        public string Code { get; set; }//refreshtoken anlamında
        public DateTime Expiration { get; set; }
    }
}
