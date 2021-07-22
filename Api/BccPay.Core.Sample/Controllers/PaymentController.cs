using BccPay.Core.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test() => Ok(new { TestConnection = true });

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePaymentRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePayment([FromBody] CreatePaymentRequest request) => Ok(request);
    }
}
