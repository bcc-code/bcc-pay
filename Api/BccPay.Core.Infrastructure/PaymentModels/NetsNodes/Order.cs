using System.Collections.Generic;
namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class Order
{
    public List<Item> Items { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; }
    public string Reference { get; set; }
}
