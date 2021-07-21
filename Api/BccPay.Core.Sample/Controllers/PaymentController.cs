using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test() => Ok(new { TestConnection = true });
    }
}
