using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BccPay.Core.Sample.Controllers
{
    public class PaymentWebhooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
