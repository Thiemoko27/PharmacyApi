using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService) {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user) {
        if(user.UserName == "admin" && user.Password == "password") {
            var token = _jwtService.GenerateToken(user.UserName, "Admin");
            return Ok(new{ Token = token });
        }

        return Unauthorized();
    }
}