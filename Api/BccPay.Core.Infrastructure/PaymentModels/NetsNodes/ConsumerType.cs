using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class ConsumerType
{
    public List<string> SupportedTypes { get; set; }

    public string Default { get; set; }
}
