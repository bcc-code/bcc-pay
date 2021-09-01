using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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
            var queryParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());

            var conditions = new List<KeyValuePair<string, string>>();
            foreach (string item in queryParams)
            {
                if (!"countryCode".Equals(item))
                {
                    conditions.Add(new KeyValuePair<string, string>(item, queryParams.Get(item)));
                }
            }

            var query = new GetCountryPaymentConfigurationsQuery(paymentConfiguration.CountryCodes, paymentConfiguration.PaymentTypes);

            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
