using BccPay.Core.Domain.EntitiesTEST;
using Raven.Client.Documents.Indexes;
using System;
using System.Linq;

namespace BccPay.Core.Infrastructure.IndexesTEST
{
    public class PaymentsIndex : AbstractMultiMapIndexCreationTask<PaymentsIndex.Result>
    {
        public PaymentsIndex()
        {
            AddMap<Payment>(payments => from payment in payments
                                        select new Result
                                        {
                                            Amount = payment.Amount,
                                            Details = payment.Details
                                        });

            Store(x => x.Amount, FieldStorage.Yes);
        }

        public class Result
        {
            public Guid PaymentId { get; set; }
            public Guid PayerId { get; set; }
            public decimal Amount { get; set; }
            public string Country { get; set; }
            public string PaymentInformation { get; set; }
            public string Details { get; set; }
        }
    }
}
