using EntitySecurity.Contract.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceTracker.Application.Common.User;
using Microsoft.AspNetCore.Http;
using ResourceTracker.Application.Constants;
using System.IdentityModel.Tokens.Jwt;

namespace ResourceTracker.Persistence.Common
{
    public class UserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            return int.Parse(GetClaimValue(CustomClaimTypes.Uid) ?? "0");
        }

        public string GetUserName()
        {
            return GetClaimValue(JwtRegisteredClaimNames.Sub);
        }

        public string GetUserEmail()
        {
            return GetClaimValue(JwtRegisteredClaimNames.Email);
        }

        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Administrator") ?? false;
        }

        public bool IsLoginAsActive()
        {
            // TODO: Implement logic to check if the user is actively logged in.
            return false;
        }

        private string GetClaimValue(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }

    }
}
