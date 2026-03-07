using System;
using System.Collections.Generic;
using System.Numerics;

namespace WebApplication1.Models;

public partial class Userdatum
{
    public long Userid { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public long Phone { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
