using ProfilesService.Application.Security;
using System.Security.Claims;

namespace ProfilesService.Api.Security
{
    public sealed class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http = http;

        private ClaimsPrincipal? User => _http.HttpContext?.User;

        public int? UserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string? Email => User?.FindFirstValue(ClaimTypes.Email);

        public string? Role => User?.FindFirstValue(ClaimTypes.Role);
    }
}
