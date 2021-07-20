using Microsoft.AspNetCore.Mvc;

namespace Bcc.Pay.Core.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test() => Ok(new { TestConnection = true });
    }
}
