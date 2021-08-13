using System;
using System.Linq;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using Raven.Client.Documents.Indexes;

namespace BccPay.Core.DataAccess.Indexes
{
    public class PaymentsIndex : AbstractMultiMapIndexCreationTask<PaymentsIndex.Result>
    {
        public PaymentsIndex()
        {
            AddMap<Payment>(payments => from payment in payments
                                        select new Result
                                        {
                                            PayerId = payment.PayerId,
                                            PaymentId = payment.PaymentId,
                                            Amount = payment.Amount,
                                            Description = payment.Description,
                                            CurrencyCode = payment.CurrencyCode,
                                            CreationDate = payment.Created,
                                            PaymentStatus = payment.PaymentStatus
                                        });
        }

        public class Result
        {
            public string CurrencyCode { get; set; }
            public string PayerId { get; set; }
            public Guid PaymentId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public DateTime CreationDate { get; set; }
        }
    }
}
