namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class WebHoorRequest
    {
        public string EventName { get; set; }
        public string Url { get; set; }
        public string Authorization { get; set; }
        /// <summary>
        /// A JArray of custom HTTP headers (name and value) to be sent with the HTTP callback request.
        /// </summary>
        public string Headers { get; set; }
    }
}