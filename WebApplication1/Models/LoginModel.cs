namespace WebApplication1.Models;

public class LoginModel
{
    public long UserId { get; set; }
    public string Password { get; set; }
    public string Token { get; set; } = string.Empty;
} 