using System.Security.Claims;

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

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    context.Items[UserIdContextKey] = userId;
                }
            }

            await _next(context);
        }
    }
}
