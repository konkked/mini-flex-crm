namespace MiniFlexCrmApi.Api.Auth;

public interface IEmailSender
{
    Task SendVerificationEmailAsync(string email, string token);
}