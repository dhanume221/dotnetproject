using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
namespace WebApplication1.API.Logout
{
    public class LogoutClass
    {
        public async Task LogoutAsync(HttpContext context, IDistributedCache cache)
        {
            try
            {
               
                var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(jti))
                {
                    var key = $"jwt:{jti}";
                    try { await cache.RemoveAsync(key); }
                    catch { /* Log error, continue */ }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Logout failed: " + ex.Message);
            }
           
        }
    }
}
