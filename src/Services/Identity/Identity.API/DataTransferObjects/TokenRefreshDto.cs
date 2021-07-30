using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.DataTransferObjects
{
    public class TokenRefreshDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
