using Microsoft.EntityFrameworkCore;
using WebApplication1.API.Register;
using WebApplication1.Models;

namespace WebApplication1.API.Login
{
    public class LoginApi
    {
        public async Task<RegResponse> LoginAsync(LoginModel model, EmployeeContext db, IJwtGenerator _jwtGenerator)
        {
            try
            {

                var user = await db.Userdata.AsNoTracking().FirstOrDefaultAsync(u => u.Userid == model.UserId);
                if (user == null)
                {
                    return new RegResponse
                    {
                        status = "Failed",
                        data = new { message = "Invalid credentials" }
                    };
                }

                bool ok = PasswordHasher.Verify(model.Password, user.Password);
                if (!ok)
                {
                    return new RegResponse
                    {
                        status = "Failed",
                        data = new { message = "Invalid credentials" }
                    };
                }

                var Token = await _jwtGenerator.GenerateAndCacheTokenAsync(user.Userid);

                return new RegResponse
                {
                    status = "Success",
                    data = new
                    {
                        userid = user.Userid,
                        token = Token
                    }
                };
            }
            catch (Exception ex)
            {
                var response = new RegResponse
                {
                    status = "Failed",
                    data = new { message = ex.Message, error = ex.InnerException?.ToString() }
                };

                return response;
            }
        }

    }
}
