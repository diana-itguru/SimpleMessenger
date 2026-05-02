using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SimpleMessenger.Models;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    public static string GenerateToken(User user, string secretKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", user.Id),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: "SimpleMessenger",
            audience: "SimpleMessenger",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}