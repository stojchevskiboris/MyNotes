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
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                {
                    context.Items[UserIdContextKey] = userId;
                }
                else
                {
                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                    if (!string.IsNullOrEmpty(email))
                    {
                        var user = await userRepository.GetByEmailAsync(email);
                        if (user != null) context.Items[UserIdContextKey] = user.Id;
                    }
                }
            }

            await _next(context);
        }

        private static bool TryStoreUserId(HttpContext context, int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            context.Items[UserIdContextKey] = userId;
            return true;
        }
    }
}
