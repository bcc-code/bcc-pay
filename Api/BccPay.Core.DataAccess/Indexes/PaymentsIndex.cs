using System;
using System.Collections.Generic;
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
                                        let isProblematicPayment = payment.Attempts
                                            .Where(x => x.AttemptStatus == AttemptStatus.Successful)
                                            .Count() > 1
                                        select new Result
                                        {
                                            IsProblematicPayment = isProblematicPayment,
                                            PaymentId = payment.PaymentId,
                                            PayerId = payment.PayerId,
                                            Created = payment.Created,
                                            Updated = payment.Updated,
                                            Description = payment.Description,
                                            CountryCode = payment.CountryCode,
                                            CurrencyCode = payment.CurrencyCode,
                                            Amount = payment.Amount,
                                            PaymentStatus = payment.PaymentStatus,
                                            Attempts = payment.Attempts.Select(x => new AttemptResult
                                            {
                                                AttemptStatus = x.AttemptStatus,
                                                CountryCode = x.CountryCode,
                                                Created = x.Created,
                                                PaymentMethod = x.PaymentMethod,
                                                PaymentProvider = x.PaymentProvider
                                            }).ToList(),
                                        });

            Store(x => x.IsProblematicPayment, FieldStorage.Yes);
            Store(x => x.Attempts, FieldStorage.Yes);
        }

        public class Result
        {
            public bool IsProblematicPayment { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public string CurrencyCode { get; set; }
            public Guid PaymentId { get; set; }
            public string PayerId { get; set; }
            public DateTime Created { get; set; }
            public DateTime? Updated { get; set; }
            public string Description { get; set; }
            public string CountryCode { get; set; }
            public decimal Amount { get; set; }
            public List<AttemptResult> Attempts { get; set; }
        }

        public class AttemptResult
        {
            public PaymentMethod PaymentMethod { get; set; }
            public PaymentProvider PaymentProvider { get; set; }
            public AttemptStatus AttemptStatus { get; set; }
            public string CountryCode { get; set; }
            public DateTime Created { get; set; }
        }
    }
}
