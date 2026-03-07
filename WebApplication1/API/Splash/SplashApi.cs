using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.API.Splash
{
    public class SplashApi
    {
        public async Task<RespData> SplashAsync(EmployeeContext db, IDistributedCache cache)
        {
            var Userid = Guid.NewGuid();
            // Build token
            var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
            var expiryMinutes = 5;

            var jti = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Userid.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(claims: claims, expires: expires, signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Persist JTI to Redis (or return error)
            var cacheKey = $"jwt:{jti}";
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryMinutes)
            };

            try
            {
                await cache.SetStringAsync(cacheKey, Userid.ToString() ?? string.Empty, cacheOptions);
            }
            catch (Exception ex)
            {
                return new RespData
                {
                    status = "Error",
                    data = new { message = "Failed to persist token to cache.", details = ex.Message }
                };
            }

            return new RespData
            {
                status = "success",
                data = new { token = tokenString, expiresAt = expires.ToString("o") }
            };
        }
    }
}
