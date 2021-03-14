namespace DummyWebApp.BLL.Services.Abstraction
{
    using System.Threading.Tasks;
    using BorsaLive.Core.Models.Abstraction;

    public interface IEmailService
    {
        public Task<IResult> SendAsync(string to, string body, string subject = "");
    }
}