using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            try
            {
                var token = authHeader.Substring("Bearer ".Length);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                context.Items["UserId"] = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                context.Items["UserRole"] = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
            } catch {}
        }
        await _next(context);
    }
}