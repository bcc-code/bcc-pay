﻿using BccPay.Core.Domain.Entities;

namespace BccPay.Core.Sample.Contracts.Requests
{
    public class SamplePaymentDetails : IPaymentDetails
    {
        public string PaymentType { get; set; }
        public string MembershipId { get; set; }
    }
}