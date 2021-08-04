using BccPay.Core.Domain.Entities;
using Newtonsoft.Json;

namespace BccPay.Core.Infrastructure.Helpers
{
    public static class StatusDetailsDeserializer<T> where T : class
    {
        public static T GetStatusDetailsType(IStatusDetails sourse)
        {
            return JsonConvert.DeserializeObject<T>(
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
