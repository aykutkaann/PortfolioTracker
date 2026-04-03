using Microsoft.AspNetCore.Http;
using PortfolioTracker.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PortfolioTracker.Application.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {

        public Guid? UserId
        {
            get
            {
                var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if(Guid.TryParse(userIdClaim, out var guidUserId))
                {
                    return guidUserId;
                }

                return null;
            }
        }

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?
        .Identity?.IsAuthenticated ?? false;
    }
}
