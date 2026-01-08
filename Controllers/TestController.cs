using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.Services;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class TestController(IEmailService _service) : Controller
    {
        public async Task<IActionResult> SendEmail()
        {
            await _service.SendEmailAsync("aliyevakhanim386@gmail.com", "Test Subject", "<h1>This is a test email body.</h1>");
            return Ok("OK");
        }
    }
}
