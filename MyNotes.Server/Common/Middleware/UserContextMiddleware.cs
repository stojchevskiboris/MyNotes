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
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) && TryStoreUserId(context, userId))
                {
                    await _next(context);
                    return;
                }

                var emailClaim = context.User.FindFirst(ClaimTypes.Email);
                if (!string.IsNullOrWhiteSpace(emailClaim?.Value))
                {
                    var user = await userRepository.GetByEmailAsync(emailClaim.Value);
                    if (user != null)
                    {
                        TryStoreUserId(context, user.Id);
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
