﻿using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        public PaymentProvider PaymentProvider { get; }

        Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentProviderSettings settings);
    }
}
