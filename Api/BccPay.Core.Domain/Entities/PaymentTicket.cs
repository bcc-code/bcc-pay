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

        public Currencies BaseCurrency { get; set; }
        public Currencies DefinitionCurrency { get; set; }

        public decimal SourceCurrencyAmount { get; set; }
        public decimal DestinationCurrencyAmount { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public void Create(Currencies baseCurrency,
            Currencies definitionCurrency,
            string paymentDefinitionId,
            string payerId,
            decimal sourceCurrencyAmount,
            decimal destinationCurrencyAmount)
        {
            TicketId = Guid.NewGuid();
            Created = DateTime.UtcNow;
            PayerId = payerId;

            PaymentDefinitionId = paymentDefinitionId;
            BaseCurrency = baseCurrency;
            DefinitionCurrency = definitionCurrency;
            SourceCurrencyAmount = sourceCurrencyAmount;
            DestinationCurrencyAmount = destinationCurrencyAmount;
        }

        public void Update(Currencies baseCurrency,
            Currencies definitionCurrency,
            string paymentDefinitionId,
            decimal sourceCurrencyAmount,
            decimal destinationCurrencyAmount)
        {
            Updated = DateTime.UtcNow;

            PaymentDefinitionId = paymentDefinitionId;
            BaseCurrency = baseCurrency;
            DefinitionCurrency = definitionCurrency;
            SourceCurrencyAmount = sourceCurrencyAmount;
            DestinationCurrencyAmount = destinationCurrencyAmount;
        }
    }
}