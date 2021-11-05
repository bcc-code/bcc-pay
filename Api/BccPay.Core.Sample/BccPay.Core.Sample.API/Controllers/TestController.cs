using System;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitializeTicket([FromBody] CreatePaymentTicketRequest request)
        {
            var command = new CreatePaymentTicketCommand(request.PaymentDefinitionId, Currencies.NOK, request.PayerId, request.CountryCode);

            var result = await Mediator.Send(command);

            return Ok(new {TicketId = result});
        }

        [HttpPut("{ticketId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentTicketResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTicket([FromRoute] Guid ticketId,
            [FromBody] UpdatePaymentTicketRequest request)
        {
            var command =
                new UpdatePaymentTicketCommand(ticketId, request.BaseCurrencyAmount, request.OtherCurrencyAmount);

            var result = await Mediator.Send(command);

            return Ok(result);
        }
    }
    public record UpdatePaymentTicketRequest(decimal? BaseCurrencyAmount, decimal? OtherCurrencyAmount);
    public record CreatePaymentTicketRequest(string PaymentDefinitionId, Currencies BaseCurrency, string PayerId,
        string CountryCode);
}