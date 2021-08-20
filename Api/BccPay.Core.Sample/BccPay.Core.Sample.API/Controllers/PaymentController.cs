﻿using System;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Sample.Contracts.Requests;
using BccPay.Core.Sample.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BccPay.Core.Sample.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : BaseController
    {
        [HttpGet("filters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPaymentWithFiltersResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProblematicPayments([FromQuery] PaymentFilters filters)
        {
            var query = new GetPaymentsWithFiltersQuery(filters.Page,
                filters.Size,
                filters.From,
                filters.To,
                filters.PaymentStatus,
                filters.IsProblematicPayment);

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentsResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPayments()
        {
            var query = new GetPaymentsQuery();

            var result = await Mediator.Send(query);

            return Ok(result);
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

            if (HttpContext.Request.Headers.TryGetValue("is-mobile-app", out var isMobileApp))
            {
                if (Boolean.TryParse(isMobileApp, out bool isMobile))
                    command.IsMobile = isMobile;
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
}
