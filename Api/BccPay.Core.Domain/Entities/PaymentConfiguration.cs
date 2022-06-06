namespace BccPay.Core.Domain.Entities;

public class PaymentConfiguration
{
    public static string GetDocumentId(string code) => $"payment-configurations/{code}";

    public string Id => GetDocumentId(CountryCode);

    public string CountryCode { get; init; }

    public PaymentConditions Conditions { get; set; }

    public string[] PaymentProviderDefinitionIds { get; set; }

    public PaymentConfiguration()
    {

    }
}

public class PaymentConditions
{
    public string[] PaymentTypes { get; set; }
    public string[] CurrencyCodes { get; set; }
}
