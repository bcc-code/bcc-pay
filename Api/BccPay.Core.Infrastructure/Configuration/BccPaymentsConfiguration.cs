using System.Collections.Generic;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Configuration;

public class BccPaymentsConfiguration
{
    public List<PaymentProviderDefinitionModel> PaymentProviderDefinitions { get; set; } = new List<PaymentProviderDefinitionModel>();

    public List<PaymentConfigurationModel> PaymentConfigurations { get; set; } = new List<PaymentConfigurationModel>();
}

public class PaymentProviderDefinitionModel
{
    public string Id { get; init; }

    public PaymentProvider Provider { get; set; }

    public PaymentSettingModel Settings { get; set; }
}

public class PaymentSettingModel
{
    public decimal Markup { get; set; }
    public PaymentMethod PaymentMethod { get; init; }
    public Currencies Currency { get; init; }
    public decimal? MaximumAmount { get; set; }
    public decimal? MinimumAmount { get; set; }
}

public class PaymentConfigurationModel
{
    public string CountryCode { get; set; }
    public ConditionModel Conditions { get; set; }
    public string[] PaymentProviderDefinitionIds { get; set; }
}

public class ConditionModel
{
    public string[] PaymentTypes { get; set; }
    public string[] CurrencyCodes { get; set; }
}
