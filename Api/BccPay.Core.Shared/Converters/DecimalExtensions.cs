using System;

namespace BccPay.Core.Shared.Converters
{
    public static class DecimalExtensions
    {
        public static decimal TwoNumbersAfterFloatPoint(this decimal number)
            => decimal.Round(number, 2, MidpointRounding.AwayFromZero);
    }
}
