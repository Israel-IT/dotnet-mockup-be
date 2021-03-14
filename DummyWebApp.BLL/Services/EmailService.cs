namespace DummyWebApp.BLL.Services
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Abstraction;
    using BorsaLive.Core.Models;
    using BorsaLive.Core.Models.Abstraction;
    using Core.ResultConstants;
    using Microsoft.Extensions.Options;
    using Options;

    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            ValidateEmailOptions(emailOptions.Value);

            _emailOptions = emailOptions.Value;
            _smtpClient = new SmtpClient(_emailOptions.Host, _emailOptions.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailOptions.Email, _emailOptions.Password)
            };
        }

        public async Task<IResult> SendAsync(string to, string body, string subject = "")
        {
            try
            {
                await _smtpClient.SendMailAsync(_emailOptions.Email!, to, subject, body);

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        private static void ValidateEmailOptions(EmailOptions emailOptions)
        {
            if (string.IsNullOrWhiteSpace(emailOptions.Email))
                throw new ArgumentException($"Please specify email in {nameof(EmailOptions)}");
            if(string.IsNullOrWhiteSpace(emailOptions.Host))
                throw new ArgumentException($"Please specify host in {nameof(EmailOptions)}");
            if(string.IsNullOrWhiteSpace(emailOptions.Password))
                throw new ArgumentException($"Please specify password in {nameof(EmailOptions)}");
            if(emailOptions.Port > 65535 || emailOptions.Port < 0)
                throw new ArgumentException($"Please specify valid port in {nameof(EmailOptions)}");
        }
    }
}