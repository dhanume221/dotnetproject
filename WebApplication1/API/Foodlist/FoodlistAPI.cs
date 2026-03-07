using Microsoft.EntityFrameworkCore;
using WebApplication1.API.Register;
using WebApplication1.Models;

namespace WebApplication1.API.Foodlist
{
    public class FoodlistAPI
    {

        private readonly IJwtGenerator _jwtGenerator;
        public FoodlistAPI(IJwtGenerator jwtGenerator) 
        {
            _jwtGenerator = jwtGenerator;
        }
        public async Task<RegResponse> DashboardAsync(long userid, string token, EmployeeContext db)
        {
            try
            {

                var data = await db.Userdata.AsNoTracking().FirstOrDefaultAsync(u => u.Userid == userid);
                if (data == null)
                {
                    return new RegResponse
                    {
                        status = "Failed",
                        data = new { message = "Invalid credentials" }
                    };
                }


                var Token = await _jwtGenerator.GenerateAndCacheTokenAsync(data.Userid);

                return new RegResponse
                {
                    status = "Success",
                    data = new
                    {
                        data = new { userid = data.Userid, name = data.Name, email = data.Email, phone = data.Phone, token = Token },

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
