using System;

namespace BccPay.Core.Shared.Converters;

public static class DecimalExtensions
{
    public static decimal ToAmountOfDigitsAfterPoint(this decimal number, int amount = 2)
        => decimal.Round(number, amount, MidpointRounding.AwayFromZero);
    
    public static decimal ToAmountOfDigitsAfterPoint(this decimal? number, int amount = 2)
        => decimal.Round(number.GetValueOrDefault(), amount, MidpointRounding.AwayFromZero);
}
