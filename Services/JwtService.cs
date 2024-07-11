using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PharmacyApi.Services;

public class JwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiruMinutes;

    public JwtService(IConfiguration config) {
        _secret = config["Jwt:key"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Key not found in configuration");

        if(_secret.Length < 32) {
            throw new ArgumentException("Jwt:Key must be at least 32 characters long.");
        }

        _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Issuer not found in configuration");
        _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Audience not found in configuration");
        _expiruMinutes = Convert.ToInt32(config["Jwt:ExpiryMinutes"]);
    }

    public string GenerateToken(string username, string role) {
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddMinutes(_expiruMinutes),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}