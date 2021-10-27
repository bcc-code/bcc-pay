﻿using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Dtos
{
    public record PaymentTicketResponse(
        Guid TicketId,
        Currencies BaseCurrency,
        Currencies OtherCurrency,
        decimal? BaseCurrencyAmount,
        decimal? OtherCurrencyAmount,
        decimal? ExchangeRate,
        DateTime? LastUpdate,
        string PaymentDefinition,
        string CountryCode);
}