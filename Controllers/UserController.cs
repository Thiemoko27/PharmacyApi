
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Data;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly UserDataBaseContext _userContext;

    public UserController(UserDataBaseContext userContext) {
        _userContext = userContext;
    }

    [HttpGet]
    public IEnumerable<User> GetAllUsers() {
        return _userContext.Users.ToList();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var userId = User.FindFirst("userId")?.Value;

        if (userId == null || userId != id.ToString()) {
            return Forbid();
        }

        var user = _userContext.Users.Find(id);

        if(user == null) {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult AddUser([FromBody] User user) {
        user.Password = HashPassword(user.Password);
        _userContext.Users.Add(user);
        _userContext.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser([FromBody] User updateUser, int id) {
        var user = _userContext.Users.Find(id);

        if(user == null) {
            return NotFound();
        }

        user.Password = HashPassword(updateUser.Password);
        user.UserName = updateUser.UserName;

        _userContext.Users.Update(user);
        _userContext.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult DeleteUser(int id) {
        var user = _userContext.Users.Find(id);

        if(user == null) {
            return NotFound();
        }

        _userContext.Users.Remove(user);
        _userContext.SaveChanges();

        return NoContent();
    }

    private string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
