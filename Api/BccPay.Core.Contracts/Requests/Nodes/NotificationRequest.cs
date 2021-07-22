using System.Collections.Generic;

namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class NotificationRequest
    {
        public List<WebHoorRequest> WebHoorRequests { get; set; }
    }
}