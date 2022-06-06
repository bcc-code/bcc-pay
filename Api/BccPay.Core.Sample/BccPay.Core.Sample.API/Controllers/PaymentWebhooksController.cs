using System;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Commands.Mollie;
using BccPay.Core.Cqrs.Commands.Nets;
using BccPay.Core.Infrastructure.PaymentModels.Webhooks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BccPay.Core.Sample.API.Controllers;

public class PaymentWebhooksController : BaseController
{

    [HttpPost("mollie/status/{paymentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMolliePaymentStatus([FromRoute] Guid paymentId, [FromBody] MollieWebhook request)
    {
        var result = await Mediator.Send(new UpdateMolliePaymentStatusCommand(request, paymentId));

        return result
            ? Ok()
            : BadRequest();
    }

    [HttpPost("nets/status/{paymentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateNetsPaymentStatus([FromRoute] Guid paymentId, [FromBody] NetsWebhook request)
    {
        var auth = HttpContext.Request.Headers[HeaderNames.Authorization];

        var command = new UpdateNetsPaymentStatusCommand(paymentId, auth, request);

        var result = await Mediator.Send(command);

        return result
            ? Ok()
            : BadRequest();
    }
}
