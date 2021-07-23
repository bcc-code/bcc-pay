using BccPay.Core.Contracts.Requests;
using BccPay.Core.Contracts.Responses;
using BccPay.Core.Cqrs.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            CreatePaymentCommand command = new(request.PayerId, "NOK", request.Amount, "NOR", Enums.PaymentMethod.CreditCard);

            var result = await Mediator.Send(command);

            return Ok(new CreatePaymentResponse { PaymentId = result });
        }

        [HttpGet("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPaymentById(string paymentId) => Ok();
    }
}
