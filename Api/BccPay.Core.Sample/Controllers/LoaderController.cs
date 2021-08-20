using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
{
    [ApiController]
    [Route("loaderio-c05109f0298068e20f81ebbf34edcbf1")]
    public class LoaderController 
    {
        [HttpGet]
      
        public string Loader()
        {
            return "loaderio-c05109f0298068e20f81ebbf34edcbf1";
        }
    }
}
