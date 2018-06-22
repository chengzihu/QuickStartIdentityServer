﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickStartIdentityMVC.Models
{
    public class JwtSettings
    {
        public string Issuer { get; set; }   //办法token的人
        public string Audience { get; set; } //token使用者
        public string SecreKey { get; set; } //token加密钥
    }
}
