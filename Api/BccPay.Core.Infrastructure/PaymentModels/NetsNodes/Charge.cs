using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class Charge
{
    public string ChargeId { get; set; }
    public int Amount { get; set; }
    public string Created { get; set; }
    public List<Item> OrderItems { get; set; }
}
