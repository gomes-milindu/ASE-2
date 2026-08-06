using System.Net;
using System.Net.Mail;
using WebApplication1.Service.Interface;
namespace WebApplication1.Service.Impl;


public class EmailSender : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        
        var mail = _configuration["EmailSettings:Mail"];
        var pwd = _configuration["EmailSettings:Password"];

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(mail, pwd),
            EnableSsl = true
        };

        return client.SendMailAsync(
            new MailMessage(mail, toEmail, subject, htmlMessage) { IsBodyHtml = true }
        );
    }
}

