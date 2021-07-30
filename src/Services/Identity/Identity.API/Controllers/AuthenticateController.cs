using Identity.API.DataTransferObjects;
using Identity.API.Entities;
using Identity.API.Features;
using Identity.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager , ITokenService tokenService , IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            //_tokenService = new TokenService(jwtTokenConfig);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var AccessToken = _tokenService.GenerateAccessToken(authClaims);
                var RefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = RefreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(int.Parse(_configuration["JwtTokenConfig:RefreshTokenExpiration"]));
                var result = await _userManager.UpdateAsync(user);
                if (result == null || !result.Succeeded)
                {
                    return Ok(new  { Status = "Error", Message = "User Can't Take token!" });
                }
                return Ok(new
                {
                    accessToken = AccessToken,
                    refreshToken = RefreshToken
                });
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new  { Status = "Success", Message = "User created successfully!" });
        }

        [Route("register-admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(RolesForUser.Admin))
                await _roleManager.CreateAsync(new UserRole(RolesForUser.Admin));
            if (!await _roleManager.RoleExistsAsync(RolesForUser.User))
                await _roleManager.CreateAsync(new UserRole(RolesForUser.User));

            if (await _roleManager.RoleExistsAsync(RolesForUser.Admin))
            {
                await _userManager.AddToRoleAsync(user, RolesForUser.Admin);
            }

            return Ok(new  { Status = "Success", Message = "User created successfully!" });
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenRefreshDto tokenRefreshDto)
        {
            if (tokenRefreshDto is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new  { Status = "Error", Message = "Please Tansfer AccessToken and Refreshtoken" });
            }
            string accessToken = tokenRefreshDto.AccessToken;
            string refreshToken = tokenRefreshDto.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = await _userManager.FindByNameAsync(username);
            //userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            var result = await _userManager.UpdateAsync(user);
            if (result == null || !result.Succeeded)
            {
                return Ok(new  { Status = "Error", Message = "User Can't Take token!" });
            }
            //user.RefreshToken = newRefreshToken;
            //userContext.SaveChanges();
            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            var result = await _userManager.UpdateAsync(user);
            if (result == null || !result.Succeeded)
            {
                return Ok(new { Status = "Error", Message = "Refresh Token Not Clear" });
            }
            return NoContent();
        }
    }
}
