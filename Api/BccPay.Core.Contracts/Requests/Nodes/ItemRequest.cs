namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class ItemRequest
    {
        public string Reference { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int NetTotalAmount { get; set; }
        public int GrossTotalAmount { get; set; }
    }
}