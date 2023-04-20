using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.DTOs
{
   public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //first name vs alınabilir. Ancak kullanıcı minimum şekilde sisteme giriş yapsın ki kullanıcı sıkılmasın.
    }
}
