namespace MiniFlexCrmApi.Auth;

public interface IEmailSender
{
    Task SendVerificationEmailAsync(string email, string token);
}