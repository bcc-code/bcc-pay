using System.Collections.Generic;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("payment-configurations")]
    public class PaymentConfigurationsController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PaymentConfigurationResult>))]
        public async Task<IActionResult> GetPaymentConfigurations([FromQuery] string countryCode)
        {
            var query = new GetCountryPaymentConfigurationsQuery(countryCode);

            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
