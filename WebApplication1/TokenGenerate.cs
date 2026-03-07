using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IJwtGenerator
{
    Task<string> GenerateAndCacheTokenAsync(long userId);
}

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _config;
    private readonly IDistributedCache _cache;

    public JwtGenerator(IConfiguration config, IDistributedCache cache)
    {
        _config = config;
        _cache = cache;
    }

    public async Task<string> GenerateAndCacheTokenAsync(long userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

        var newJti = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, newJti)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var expiryMinutes = 5;
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var newToken = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = tokenHandler.WriteToken(newToken);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryMinutes)
        };

        // If Redis fails, just return token (caller decides)
        await _cache.SetStringAsync($"jwt:{newJti}", userId.ToString(), cacheOptions);

        return tokenString;
    }
}
