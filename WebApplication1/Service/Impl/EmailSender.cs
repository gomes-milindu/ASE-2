using System.Net;
using System.Net.Mail;
using WebApplication1.Service.Interface;



    public class EmailSender : IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        
        var mail = "kanishkagom@gmail.com";
        var pwd = "yrqzglqquddawphz";

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

