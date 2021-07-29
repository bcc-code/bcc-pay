using BccPay.Core.Contracts.Requests;
using BccPay.Core.Contracts.Responses;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Cqrs.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : BaseController
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatePaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            CreatePaymentCommand command = Mapper.Map<CreatePaymentCommand>(request);

            var result = await Mediator.Send(command);

            return Ok(new CreatePaymentResponse { PaymentId = result });
        }

        [HttpPost("{paymentId}/attempts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatePaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePaymentAttempt(Guid paymentId, [FromBody] CreatePaymentAttemptRequest request)
        {
            CreatePaymentAttemptCommand command = Mapper.Map<CreatePaymentAttemptCommand>(request);
            command.PaymentId = paymentId;

            var result = await Mediator.Send(command);

            return Ok(new CreatePaymentResponse { PaymentId = result });
        }

        [HttpGet("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPayments(Guid paymentId)
        {
            var query = new GetPaymentByIdQuery(paymentId);

            var result = await Mediator.Send(query);

            return Ok(Mapper.Map<GetPaymentResponse>(result));
        }
    }
}
