using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public class PaymentTicket
    {
        public static string GetDocumentId(Guid ticketId)
            => $"payment-tickets/{ticketId}";

        public string Id => GetDocumentId(TicketId);

        public Guid TicketId { get; set; }

        public string PaymentDefinitionId { get; set; }
        public string PayerId { get; set; }
        public string CountryCode { get; set; }

        public Currencies BaseCurrency { get; set; }
        public Currencies DefinedCurrency { get; set; }

        public decimal? BaseCurrencyAmount { get; set; }
        public decimal? OtherCurrencyAmount { get; set; }
        public decimal ExchangeRate { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public TicketStatus TicketStatus { get; set; }

        public void Create(Currencies baseCurrency,
            Currencies definedCurrency,
            string paymentDefinitionId,
            string payerId,
            string countryCode,
            decimal exchangeRate)
        {
            TicketId = Guid.NewGuid();
            Created = DateTime.UtcNow;
            TicketStatus = TicketStatus.Initialized;
            PayerId = payerId;

            PaymentDefinitionId = paymentDefinitionId;
            BaseCurrency = baseCurrency;
            CountryCode = countryCode;
            DefinedCurrency = definedCurrency;
            ExchangeRate = exchangeRate;
        }

        private bool IsTicketExpired()
        {
            if (!(Updated < DateTime.UtcNow.AddHours(-1)))
                return false;

            TicketStatus = TicketStatus.Expired;
            return true;
        }

        public void Update(
            bool isOpposite,
            decimal inputAmount,
            decimal outputAmount,
            decimal exchangeRate)
        {
            Updated = DateTime.UtcNow;

            if (IsTicketExpired())
                return;

            TicketStatus = TicketStatus.Stored;

            if (isOpposite)
            {
                BaseCurrencyAmount = outputAmount;
                OtherCurrencyAmount = inputAmount;
            }
            else
            {
                BaseCurrencyAmount = inputAmount;
                OtherCurrencyAmount = outputAmount;
            }

            ExchangeRate = exchangeRate;
        }
    }
}