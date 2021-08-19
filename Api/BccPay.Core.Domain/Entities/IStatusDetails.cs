using System.Collections.Generic;

namespace BccPay.Core.Domain.Entities
{
    public interface IStatusDetails
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}