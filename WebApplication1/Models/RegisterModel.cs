using System.Numerics;

namespace WebApplication1.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; } = null;
        public string Password { get; set; }
        public string Username { get; set;} = null;
        public long Phone { get; set;}
        public string Token { get; set; } = string.Empty;

    }
}
