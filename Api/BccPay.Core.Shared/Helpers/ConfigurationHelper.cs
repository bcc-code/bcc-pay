using System.Reflection;
using System.Text;

namespace BccPay.Core.Shared.Helpers
{
    public static class ConfigurationHelper
    {
        public static bool IsAnyFieldNullOrEmpty(this object incomingObject, out string objectEmptyFields)
        {
            StringBuilder stringBuilder = new(string.Empty);

            foreach (PropertyInfo propertyInformation in incomingObject.GetType().GetProperties())
            {
                if (propertyInformation.PropertyType == typeof(string))
                {
                    string propertyValue = (string)propertyInformation.GetValue(incomingObject);
                    if (string.IsNullOrWhiteSpace(propertyValue))
                    {
                        stringBuilder.Append(propertyInformation.Name + ' ');
                    }
                }
            }

            objectEmptyFields = stringBuilder.ToString();
            if (string.IsNullOrWhiteSpace(objectEmptyFields))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
