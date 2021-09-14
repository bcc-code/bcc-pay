﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Sample.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.API.Controllers
{
    [ApiController]
    [Route("payment-configurations")]
    public class PaymentConfigurationsController : BaseController
    {
        private readonly ICurrencyService _exchangeService;

        public PaymentConfigurationsController(ICurrencyService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentConfigurationResult>))]
        public async Task<IActionResult> GetPaymentConfigurations([FromQuery] PaymentConfigurationRequest paymentConfiguration)
        {
            var query = new GetPaymentConfigurationsByQuery(paymentConfiguration.CountryCode, paymentConfiguration.PaymentType, paymentConfiguration.CurrencyCode);

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("exchange")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExchangeResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromQuery] Currencies currency, decimal amount, string providerDefinitionId)
        {
            var query = new GetExchangedCurrencyQuery(amount, currency, providerDefinitionId);

            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
