using System.Collections.Generic;

namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class OrderRequest
    {
        public List<ItemRequest> Items { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}
