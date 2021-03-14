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
    using Microsoft.Extensions.Configuration;

    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"])
            };
        }

        public async Task<IResult> SendAsync(string to, string body, string subject = "")
        {
            try
            {
                await _smtpClient.SendMailAsync(_configuration["EmailSettings:Email"], to, subject, body);

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}