using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BccPay.Core.Shared.Helpers;

public static class TemplateHelper
{
    public static string ProcessPlaceholders<T>(string text, T model, Regex pattern)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        MatchCollection matches = pattern.Matches(text);
        IEnumerable<string> tokens = matches.Cast<Match>().Select(m => m.Groups["token"].Value).Distinct();
        var publicProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var token in tokens)
        {
            var property = publicProperties.FirstOrDefault(p => string.Equals(p.Name, token, StringComparison.InvariantCultureIgnoreCase));
            if (property is not null && property.CanRead)
            {
                object valueToReplace = property.GetValue(model) ?? string.Empty;
                Regex replaceRegex = new(@"\{\{\s*" + token + @"\s*\}\}");
                text = replaceRegex.Replace(text, valueToReplace.ToString());
            }
        }

        return text;
    }
}
