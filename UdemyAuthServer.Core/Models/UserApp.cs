using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Models
{
    //IdentityUseri eklemek için nuget package kısmından Microsoft.AspnetCore.identity.EntityFrameworkCore kütüphanesini ekledik.
   public class UserApp:IdentityUser //identityUser içinde varsayılan olarak tablo sütunları gelmektedir. (property)
    {
        public string City { get; set; }
    }
}
