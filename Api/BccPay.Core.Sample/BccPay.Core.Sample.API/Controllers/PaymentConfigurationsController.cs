using System.Collections.Generic;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Sample.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.API.Controllers
{
    [ApiController]
    [Route("payment-configurations")]
    public class PaymentConfigurationsController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentConfigurationResult>))]
        public async Task<IActionResult> GetPaymentConfigurations([FromQuery] PaymentConfigurationRequest paymentConfiguration)
        {
            var query = new GetPaymentConfigurationsByQuery(paymentConfiguration.CountryCode, paymentConfiguration.PaymentType, paymentConfiguration.CurrencyCode);

            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
