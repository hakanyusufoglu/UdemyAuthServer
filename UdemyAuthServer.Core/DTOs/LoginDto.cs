﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.DTOs
{
   public class LoginDto 
    {
        public string Email { get; set; } //client id 
        public string Password { get; set; } //client secret gibi düşün.
    }
}
