using System.Collections.Generic;

namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class ConsumerTypeRequest
    {
        public List<string> SupportedTypes { get; set; }
        public string Default { get; set; }
    }
}