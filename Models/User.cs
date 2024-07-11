using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
 
    [Required(ErrorMessage = "User name is required")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}