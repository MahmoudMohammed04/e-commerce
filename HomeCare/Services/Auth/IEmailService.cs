namespace HomeCare.Services.Auth
{
    public interface IEmailService
    {
        public void SendEmail(string to, string subject, string body);

        public void SendEmailConfirmation(string to, string userId, string token);

        public void SendPasswordReset(string to,  string token);
    }
}
