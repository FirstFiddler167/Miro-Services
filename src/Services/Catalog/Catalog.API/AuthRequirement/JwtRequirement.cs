using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Catalog.API.AuthRequirement
{
    public class JwtRequirement : IAuthorizationRequirement { }
    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _client;
        private readonly HttpContext _httpContext;
        private readonly IConfiguration _configuration;

        public JwtRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor , 
            IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient("IdentityClient") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];
                _client.BaseAddress = new Uri(_configuration["JwtTokenConfig:IdentityServerUrl"]);
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                var response = await _client
                    .GetAsync($"{_configuration["JwtTokenConfig:ValidateTokenController"]}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //_httpContext.User.
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
        }
    }


}
