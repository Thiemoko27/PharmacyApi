using Microsoft.AspNetCore.Identity.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using PharmacyApi.Data;
using PharmacyApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace PharmacyApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserDataBaseContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(UserDataBaseContext context, IConfiguration configuration) {
        _context = context;
        _configuration = configuration;

        if(!_context.Users.Any()) {
            var admin = new User {
                UserName = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "Admin"
            };
            _context.Users.Add(admin);
            _context.SaveChanges();
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest) {
        var user = _context.Users.SingleOrDefault(u => u.UserName == loginRequest.UserName);

        if(user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password)) {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = GenerateJwtToken(user);

        return Ok(new{token, role = user.Role});
    }

    private string GenerateJwtToken(User user) {
        var jwtkey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrEmpty(jwtkey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience)) {
                throw new ApplicationException("JWT configuration is missing or invalid.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey));
        var Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: Claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: Credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}