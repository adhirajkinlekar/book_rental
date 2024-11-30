using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace book_rental_api.Controllers
{
    public abstract class APIControllerBase : Controller
    {
        protected readonly Guid? UserId;

        public APIControllerBase(IHttpContextAccessor httpContextAccessor)
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            UserId = userIdClaim != null ? Guid.TryParse(userIdClaim, out var parsedGuid) ? parsedGuid : (Guid?)null : null;
        }
    }
}
