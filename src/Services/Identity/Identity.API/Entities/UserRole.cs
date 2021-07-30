using Identity.API.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Entities
{
    public class UserRole : IdentityRole, IBaseEntity
    {
        public UserRole(string roleName)
        {
            CreatedBy = "Admin";
            CreatedDate = DateTime.Now;
            NormalizedName = roleName.ToUpper();
            Name = roleName;

        }
        public UserRole()
        {
            CreatedBy = "Admin";
            CreatedDate = DateTime.Now;

        }
        public string CreatedBy { get ; set ; }
        public DateTime CreatedDate { get ; set ; }
        public string LastModifiedBy { get ; set ; }
        public DateTime LastModifiedDate { get ; set ; }
    }
}
