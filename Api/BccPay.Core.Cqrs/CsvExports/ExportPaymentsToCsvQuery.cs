using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace BccPay.Core.Cqrs.CsvExports
{
    internal static class ExportPaymentsToCsvQuery
    {
        public static byte[] CreateCSVEncodedPaymentResults(List<NormalizePayment> payments)
        {
            var csvModels = payments.Select(payment =>
                new PaymentInCsvModel(
                    PaymentId: payment.PaymentId,
                    PayerId: payment.PayerId,
                    PaymentDetails: payment.PaymentDetails,
                    CurrencyCode: payment.CurrencyCode,
                    Amount: payment.Amount,
                    Created: payment.Created,
                    Updated: payment.Updated,
                    PaymentStatus: payment.PaymentStatus,
                    IsProblematicPayment: payment.IsProblematicPayment,
                    Attempt: payment.Attempt is null
                        ? null
                        : new AttemptInCsvModel(
                            AttemptId: payment.Attempt.AttemptId,
                            IsSuccessfulAttempt: payment.Attempt.IsSuccessfulAttempt,
                            AttemptStatus: payment.Attempt.AttemptStatus,
                            PaymentMethod: payment.Attempt.PaymentMethod,
                            DetailsJson: payment.Attempt.DetailsJson))).ToList();

            using MemoryStream fileStream = new();
            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            using (CsvWriter csvWriter = new(streamWriter,
                    new CsvConfiguration(CultureInfo.GetCultureInfo("nb-NO"))
                    {
                        Delimiter = ";",
                        IgnoreBlankLines = false
                    }))
            {
                csvWriter.Context.RegisterClassMap<PaymentCsvMapper>();
                csvWriter.Context.RegisterClassMap<AttemptCsvMapper>();
                csvWriter.WriteHeader<PaymentInCsvModel>();
                csvWriter.NextRecord();

                foreach (PaymentInCsvModel csvModel in csvModels)
                {
                    csvWriter.WriteRecord(csvModel);
                    csvWriter.NextRecord();
                }
            }
            return fileStream.ToArray();
        }
    }

    internal record PaymentInCsvModel(
        Guid PaymentId,
        string PayerId,
        string PaymentDetails,
        string CurrencyCode,
        decimal Amount,
        DateTime Created,
        DateTime? Updated,
        string PaymentStatus,
        bool IsProblematicPayment,
        AttemptInCsvModel Attempt);

    internal record AttemptInCsvModel(
        bool IsSuccessfulAttempt,
        Guid AttemptId,
        string AttemptStatus,
        string PaymentMethod,
        string DetailsJson);

    internal sealed class PaymentCsvMapper : ClassMap<PaymentInCsvModel>
    {
        public PaymentCsvMapper()
        {
            Map(paymentModel => paymentModel.PaymentId).Name("Payment ID");
            Map(paymentModel => paymentModel.PayerId).Name("Payer ID");
            Map(paymentModel => paymentModel.PaymentDetails).Name("Payment details in JSON");
            Map(paymentModel => paymentModel.CurrencyCode).Name("Currency Code");
            Map(paymentModel => paymentModel.Amount).Name("Amount value");
            Map(paymentModel => paymentModel.Created).Name("Payment creation date");
            Map(paymentModel => paymentModel.Updated).Name("Payment last update");
            Map(paymentModel => paymentModel.PaymentStatus).Name("Payment status");
            Map(paymentModel => paymentModel.IsProblematicPayment).Name("Is payment problematic");
            References<AttemptCsvMapper>(attempt => attempt.Attempt);
        }
    }

    internal sealed class AttemptCsvMapper : ClassMap<AttemptInCsvModel>
    {
        public AttemptCsvMapper()
        {
            Map(attemptModel => attemptModel.IsSuccessfulAttempt).Name("Is attempt successful");
            Map(attemptModel => attemptModel.AttemptId).Name("Attempt ID");
            Map(attemptModel => attemptModel.AttemptStatus).Name("Attempt Status");
            Map(attemptModel => attemptModel.PaymentMethod).Name("Attempt Method");
            Map(attemptModel => attemptModel.DetailsJson).Name("Attempt details in JSON");
        }
    }
}
