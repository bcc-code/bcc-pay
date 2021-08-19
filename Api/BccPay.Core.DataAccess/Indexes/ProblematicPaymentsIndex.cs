using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using Raven.Client.Documents.Indexes;

namespace BccPay.Core.DataAccess.Indexes
{
    public class ProblematicPaymentsIndex : AbstractMultiMapIndexCreationTask<ProblematicPaymentsIndex.Result>
    {
        public ProblematicPaymentsIndex()
        {
            AddMap<Payment>(payments => from payment in payments
                                        where payment.Attempts
                                            .Where(x => x.AttemptStatus == Enums.AttemptStatus.PaymentIsSuccessful).Count() > 1
                                        select new Result
                                        {
                                            PaymentId = payment.PaymentId,
                                            PayerId = payment.PayerId,
                                            Created = payment.Created,
                                            Updated = payment.Updated,
                                            Description = payment.Description,
                                            CountryCode = payment.CountryCode,
                                            CurrencyCode = payment.CurrencyCode,
                                            Amount = payment.Amount,
                                            Attempts = payment.Attempts.Select(x => new AttemptResult
                                            {
                                                AttemptStatus = x.AttemptStatus,
                                                CountryCode = x.CountryCode,
                                                Created = x.Created,
                                                PaymentMethod = x.PaymentMethod,
                                                PaymentProvider = x.PaymentProvider
                                            }).ToList(),
                                        });

            Store(x => x.Attempts, FieldStorage.Yes);
        }

        public class Result
        {
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
