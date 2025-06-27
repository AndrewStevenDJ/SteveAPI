namespace SteveAPI.Models;

public class User
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;   // plain-text demo; usa hash real en producción
    public string Role     { get; set; } = "User";
}
