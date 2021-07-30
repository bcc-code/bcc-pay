namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class InvoiceDetails
    {
        public string InvoiceNumber { get; set; }
        public string Orc { get; set; }
        public string PdfLink { get; set; }
        public string DueDate { get; set; }
    }
}