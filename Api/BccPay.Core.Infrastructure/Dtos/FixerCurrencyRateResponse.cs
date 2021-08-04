namespace BccPay.Core.Infrastructure.Dtos
{
    public class FixerCurrencyRateResponse
    {
        public bool Success { get; set; }
        public string Timestamp { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Rates Rates { get; set; }
    }

    public class Rates
    {
        public string EUR { get; set; }
        public string USD { get; set; }
    }
}
