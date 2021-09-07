using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace BccPay.Core.Cqrs.Queries
{
    public static class ExportPaymentsToCsvQuery
    {
        public static byte[] CreateCSVEncodedPaymentResults(List<PaymentResult> payments)
        {
            var csvModels = payments.Select(payment =>
                new PaymentResultModel(Description: payment.Description)).ToList();

            using MemoryStream fileStream = new();
            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            using (CsvWriter csvWriter = new(streamWriter, new CsvConfiguration(
                System.Globalization.CultureInfo.GetCultureInfo("nb-NO"))
            { Delimiter = ";", IgnoreBlankLines = false }))
            {
                csvWriter.Context.RegisterClassMap<PaymentCsvMapper>();
                csvWriter.WriteHeader<PaymentResultModel>();
                csvWriter.NextRecord();

                foreach (PaymentResultModel csvModel in csvModels)
                {
                    csvWriter.WriteRecord(csvModel);
                    csvWriter.NextRecord();
                }
            }
            return fileStream.ToArray();
        }
    }

    public record PaymentResultModel([Name("Description")] string Description);

    public sealed class PaymentCsvMapper : ClassMap<PaymentResultModel>
    {
        public PaymentCsvMapper()
        {
            Map(paymentModel => paymentModel.Description)
                .Name("Description");
        }
    }
}
