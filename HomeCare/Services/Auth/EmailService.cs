using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using HomeCare.Services.Url;
using System.Net;


namespace HomeCare.Services.Auth
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IUrlService _urlService;
        public EmailService(IConfiguration config,IUrlService urlService)
        {
            _config = config;
            _urlService = urlService;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var email = _config["Email:User"];
            var password = _config["Email:Password"];

           

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = body };

            
            using var client = new SmtpClient();
            try
            {
                client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(email, password);
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void SendEmailConfirmation(string to,string userId,string token)
        {
            var url = _urlService.GetBaseUrl();
            //var url = "https://nonevaporative-distended-terrance.ngrok-free.dev";
            token = WebUtility.UrlEncode(token);
            userId = WebUtility.UrlEncode(userId);
            var subject = "Email confirmation";
            var body = $"Please confirm your email by clicking the link: {url}Auth/confirm-email?token={token}&userId={userId}";

            SendEmail(to, subject, body);
        }

        public void SendPasswordReset(string to, string token)
        {
            var url = _urlService.GetBaseUrl();
            token = WebUtility.UrlEncode(token);
            var subject = "Password reset";
            var body = $"Your Reset Code is: {token}";

            SendEmail(to, subject, body);
        }
    }
}
