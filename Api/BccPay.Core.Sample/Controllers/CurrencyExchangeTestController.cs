using System.Threading.Tasks;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromQuery] Currencies from, Currencies to, decimal amount, decimal tax)
        {
            var result = await _exchangeService.Exchange(from, to, amount, tax);

            return Ok(new { ConvertResult = result.Item1, Tax = result.Item2 });
        }
    }
}
