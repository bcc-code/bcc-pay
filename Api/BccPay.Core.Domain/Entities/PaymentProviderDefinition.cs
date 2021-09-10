﻿using BccPay.Core.Enums;

namespace BccPay.Core.Domain
{
    public class PaymentProviderDefinition
    {
        public static string GetDocumentId(string code) => $"payment-provider-definitions/{code}";

        public string Id => GetDocumentId(PaymentDefinitionCode);

        public string PaymentDefinitionCode { get; set; }

        public PaymentProvider Provider { get; set; }

        public PaymentSettings Settings { get; set; }
    }

    public class PaymentSettings
    {
        public PaymentMethod PaymentMethod { get; init; }

        public Currencies Currency { get; init; }
    }
}