using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Broadcaster.Business;
using Microsoft.IdentityModel.Tokens;

namespace Broadcaster.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UserObject user)
    {
        var secretKey = _configuration["Jwt:SecurityKey"]
            ?? throw new InvalidOperationException("Jwt:SecurityKey is missing");

        var issuer = _configuration["Jwt:Issuer"] ?? "https://localhost";
        var audience = _configuration["Jwt:Audience"] ?? "https://localhost";
        var lifetimeMinutes = int.Parse(_configuration["Jwt:AccessTokenLifetimeMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.GoogleSub),   // Unique Google user ID
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("google_sub", user.GoogleSub),                // Custom claim for easy lookup
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
            new Claim(ClaimTypes.Role, user.RolesAsString)
        };

        // Add your role mapping logic here if needed
        // var roles = RoleMapping.GetRolesFromGoogleUser(...);
        // foreach (var role in roles)
        //     claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}