// unset

using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.API.Controllers;

[ApiController]
[Route("loaderio-c05109f0298068e20f81ebbf34edcbf1")]
public class LoadTestController : Controller
{
    [HttpGet]
    public string Loader()
    {
        return "loaderio-c05109f0298068e20f81ebbf34edcbf1";
    }
}
