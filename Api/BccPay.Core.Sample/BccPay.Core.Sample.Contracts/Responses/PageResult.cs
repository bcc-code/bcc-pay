namespace BccPay.Core.Sample.Contracts.Responses
{
    public class PageResult<TSource> where TSource : class
    {
        public int AmountOfPages { get; set; }
        public int AmountOfObjects { get; set; }
        public TSource Data { get; set; }
    }
}
