﻿namespace BccPay.Core.Domain.Entities
{
    public interface IStatusDetails
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }
}