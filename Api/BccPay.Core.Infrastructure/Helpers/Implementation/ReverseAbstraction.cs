using Newtonsoft.Json;

namespace BccPay.Core.Infrastructure.Helpers
{
    public static class ReverseAbstraction<Implementation, Abstraction>
        where Implementation : class
        where Abstraction : class
    {
        public static Implementation GetImplementationFromAbstraction(Abstraction sourse)
        {
            return JsonConvert.DeserializeObject<Implementation>(
                        JsonConvert.SerializeObject(sourse, Formatting.Indented, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            NullValueHandling = NullValueHandling.Ignore
                        }),
                   new JsonSerializerSettings
                   {
                       TypeNameHandling = TypeNameHandling.Auto,
                       NullValueHandling = NullValueHandling.Ignore
                   });
        }
    }
}
