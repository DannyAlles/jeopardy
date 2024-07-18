using Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IProfessorService professorService, IAuthenticationService authenticationService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = authenticationService.ValidateToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                var inspector = await professorService.GetById(userId.Value).ConfigureAwait(false);
                context.Items["User"] = inspector;
            }

            await _next(context);
        }
    }
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
