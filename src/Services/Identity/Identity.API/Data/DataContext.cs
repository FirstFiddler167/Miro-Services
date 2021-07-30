using Identity.API.DataTransferObjects;
using Identity.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser, UserRole, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUsers(builder);
            SeedRoles(builder);
            SeedUserRoles(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                Email = "The.First.Fiddler167@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "+201066442515"
            };
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "P@ssw0rd");
            builder.Entity<ApplicationUser>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<UserRole>().HasData(
                new UserRole(RolesForUser.Admin) { Id = "fab4fac1-c546-41de-aebc-a14da6895711", ConcurrencyStamp = "1"},
                new UserRole(RolesForUser.User) { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", ConcurrencyStamp = "2" }
                );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
                );
        }

    }
}
