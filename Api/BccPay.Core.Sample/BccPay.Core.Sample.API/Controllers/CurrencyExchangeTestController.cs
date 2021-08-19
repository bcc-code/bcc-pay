using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyExchangeTestController : BaseController
    {
        private readonly ICurrencyService _exchangeService;
        public CurrencyExchangeTestController(ICurrencyService exchangeService)
        {
            _exchangeService = exchangeService;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyConversionRecord))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromQuery] Currencies from, Currencies to, decimal amount, decimal exchangeRateMarkup)
        {
            var result = await _exchangeService.Exchange(from, to, amount, exchangeRateMarkup);

            return Ok(result);
        }
    }
}
