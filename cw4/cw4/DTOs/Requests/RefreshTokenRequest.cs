using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DTOs.Requests
{
    public class RefreshTokenRequest
    {
        public string Login { get; set; }
        public string RefreshToken { get; set; }
    }
}
