using WebApplication1.Models;


namespace WebApplication1.API.Register
{
    public class RegisterApi
    {
        private readonly EmailService _emailService;

        public RegisterApi(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<RegResponse> RegisterAsync(RegisterModel reg, EmployeeContext db, IJwtGenerator _jwtGenerator)
        {
            try
            {
                // Passed token validation — proceed to register user
                var regdata = new Userdatum
                {
                    Userid = Convert.ToInt64(reg.Phone),
                    Name = reg.Name,
                    Email = reg.Email,
                    Password = PasswordHasher.Hash(reg.Password),
                    Username = reg.Username,
                    Phone = reg.Phone
                };


                db.Userdata.Add(regdata);
                await db.SaveChangesAsync();

                // Issue a new token for the newly registered user and store its JTI in Redis
                await _emailService.SendEmailAsync(reg.Email, "Welcome to Our Service", $"Hello {reg.Name}, Thank you for registering!");
                var tokenString = await _jwtGenerator.GenerateAndCacheTokenAsync(regdata.Userid);

                var response = new RegResponse
                {
                    status = "Success",
                    data = new { name = regdata.Name, phone = regdata.Phone, email = regdata.Email, token = tokenString }
                };

                return response;
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