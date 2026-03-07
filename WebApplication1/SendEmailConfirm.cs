using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApplication1;

public class EmailService
{
    private readonly IConfiguration _configuration;

    private const string SmtpServer = "smtp.gmail.com";

    private readonly EmailcredModel _creds;

    public EmailService(IOptions<EmailcredModel> credsOptions)
    {
        _creds = credsOptions.Value;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_creds.Id));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Text) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(SmtpServer, 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_creds.Id, _creds.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
