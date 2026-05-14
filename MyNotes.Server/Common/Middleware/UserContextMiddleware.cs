using System.Security.Claims;
using MyNotes.Server.Data.Interfaces;

namespace MyNotes.Server.Common.Middleware
{
    public class UserContextMiddleware
    {
        public const string UserIdContextKey = "UserId";
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) && userId > 0)
                {
                    context.Items[UserIdContextKey] = userId;
                }
                else
                {
                    var emailClaim = context.User.FindFirst(ClaimTypes.Email);
                    if (!string.IsNullOrWhiteSpace(emailClaim?.Value))
                    {
                        var user = await userRepository.GetByEmailAsync(emailClaim.Value);
                        if (user != null && user.Id > 0)
                        {
                            context.Items[UserIdContextKey] = user.Id;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
