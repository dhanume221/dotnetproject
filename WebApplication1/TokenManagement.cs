using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface ITokenValidator
{
    ClaimsPrincipal? Validate(string token);
}

public class TokenValidator : ITokenValidator
{
    private readonly string _key;
    public TokenValidator(IConfiguration config)
    {
        _key = Environment.GetEnvironmentVariable("JWTKEY");
        
    }

    public ClaimsPrincipal? Validate(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null; // invalid
        }
    }
}

