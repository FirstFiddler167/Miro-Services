using Identity.API.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Entities
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string CreatedBy { get ; set ; }
        public DateTime CreatedDate { get; set ; }
        public string LastModifiedBy { get ; set ; }
        public DateTime LastModifiedDate { get ; set ; }
    }
}
