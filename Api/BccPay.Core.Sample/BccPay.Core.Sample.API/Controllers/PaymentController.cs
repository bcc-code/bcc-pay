using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Sample.Contracts.Requests;
using BccPay.Core.Sample.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BccPay.Core.Sample.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : BaseController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageResult<List<PaymentResult>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentFilters filters)
    {
        var query = new GetPaymentsWithFiltersQuery(filters.Page,
            filters.Size,
            filters.From,
            filters.To,
            filters.PaymentStatus,
            filters.IsProblematicPayment,
            filters.PaymentType,
            filters.PayerId);

        var result = await Mediator.Send(query);

        return Ok(new PageResult<List<PaymentResult>>
        {
            AmountOfObjects = result.AmountOfObjects,
            AmountOfPages = filters.Size == 0 ? 1 : result.AmountOfObjects / filters.Size,
            Data = result.Payments
        });
    }

    [HttpGet("problematic-count")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProblematicPaymentsCountResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProblematicPaymentsCount()
    {
        var query = new GetProblematicPaymentsCountQuery();

        var result = await Mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentsCsv([FromQuery] PaymentFilters filters)
    {
        var query = new GetPaymentsBase64CsvQuery(
            filters.From,
            filters.To,
            filters.PaymentStatus,
            filters.IsProblematicPayment,
            filters.PaymentType,
            filters.PayerId);

        var result = await Mediator.Send(query);

        return Ok(new { csv = result });
    }

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaymentAttemptResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaymentAttempt(Guid paymentId, [FromBody] CreatePaymentAttemptRequest request)
    {
        CreatePaymentAttemptCommand command = Mapper.Map<CreatePaymentAttemptCommand>(request);

        if (HttpContext.Request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var userLanguage))
        {
            command.AcceptLanguage = userLanguage.ToString().Substring(0, 5);
        }

        command.PaymentId = paymentId;

        var result = await Mediator.Send(command);

        var mapResult = Mapper.Map<IPaymentAttemptResponse>(result);

        return Ok(mapResult);
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
