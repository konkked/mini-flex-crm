using System.Net.Mail;

namespace MiniFlexCrmApi.Api.Auth;

public class EmailSender : IEmailSender
{private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public EmailSender(string smtpHost, int smtpPort, string senderEmail, string senderPassword)
    {
        _smtpHost = smtpHost ?? throw new ArgumentNullException(nameof(smtpHost));
        _smtpPort = smtpPort;
        _senderEmail = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail));
        _senderPassword = senderPassword ?? throw new ArgumentNullException(nameof(senderPassword));
    }

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail, "MiniFlex CRM"),
            Subject = "Verify Your Email",
            IsBodyHtml = true
        };
        mailMessage.To.Add(new MailAddress(email));

        string verificationLink = $"https://yourdomain.com/verify?token={Uri.EscapeDataString(token)}";
        mailMessage.Body = $@"<h1>Welcome to MiniFlex CRM!</h1>
                              <p>Please verify your email by clicking the link below:</p>
                              <a href=""{verificationLink}"">Verify Email</a>
                              <p>This link expires in {ServerCustomConstants.RefreshWindowInMinutes} minutes.</p>";

        using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
        {
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(_senderEmail, _senderPassword);
            await smtpClient.SendMailAsync(mailMessage); // Sends MailMessage
        }
    }
}