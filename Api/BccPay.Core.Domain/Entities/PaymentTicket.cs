using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public class PaymentTicket
    {
        public static string GetDocumentId(Guid ticketId)
            => $"paymentTickets/{ticketId}";

        public string Id => GetDocumentId(TicketId);

        public Guid TicketId { get; set; }

        public string PaymentDefinitionId { get; set; }
        public string PayerId { get; set; }
        public string CountryCode { get; set; }

        public Currencies BaseCurrency { get; set; }
        public Currencies DefinedCurrency { get; set; }

        public decimal? InputAmount { get; set; }
        public decimal? OutputAmount { get; set; }
        public decimal? ExchangeRate { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public void Create(Currencies baseCurrency,
            Currencies definedCurrency,
            string paymentDefinitionId,
            string payerId,
            string countryCode)
        {
            TicketId = Guid.NewGuid();
            Created = DateTime.UtcNow;
            PayerId = payerId;

            PaymentDefinitionId = paymentDefinitionId;
            BaseCurrency = baseCurrency;
            CountryCode = countryCode;
            DefinedCurrency = definedCurrency;
        }

        public void Update(
            decimal inputAmount,
            decimal outputAmount,
            decimal exchangeRate)
        {
            Updated = DateTime.UtcNow;
            
            InputAmount = inputAmount;
            OutputAmount = outputAmount;
            ExchangeRate = exchangeRate;
        }
    }
}