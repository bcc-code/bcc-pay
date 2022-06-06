using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;

public class MolliePaymentRequest
{
    public MollieAmount Amount { get; set; }
    /// <summary>
    /// Description will be shown to your customer on their card or bank statement when possible.
    /// Unique and required
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// url + unique
    /// </summary>
    public string RedirectUrl { get; set; }
    public string WebhookUrl { get; set; }
    /// <summary>
    /// Allows you to preset the language to be used in the hosted payment pages shown to the consumer. 
    /// Setting a locale is highly recommended and will greatly improve your conversion rate. 
    /// When this parameter is omitted, the browser language will be used instead if supported by the payment method. 
    /// You can provide any xx_XX format ISO 15897 locale, but our hosted payment pages currently only support the following languages:
    /// Possible values: en_US nl_NL nl_BE fr_FR fr_BE de_DE de_AT de_CH es_ES ca_ES pt_PT it_IT nb_NO sv_SE fi_FI da_DK is_IS hu_HU pl_PL lv_LV lt_LT
    /// </summary>
    public string Locale { get; set; }
    /// <summary>
    /// Possible values: applepay bancontact banktransfer belfius creditcard directdebit eps giftcard giropay ideal kbc mybank paypal paysafecard przelewy24 sofort
    /// </summary>
    public string[] Method { get; set; }
    public object Links { get; set; }
}
