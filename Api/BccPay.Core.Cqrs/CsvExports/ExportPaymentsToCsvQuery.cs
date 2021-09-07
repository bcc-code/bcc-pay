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
                    MembershipId: payment.MembershipId,
                    PaymentType: payment.PaymentType,
                    CurrencyCode: payment.CurrencyCode,
                    Amount: payment.Amount,
                    Created: payment.Created,
                    Updated: payment.Updated,
                    PaymentStatus: Enum.GetName(payment.PaymentStatus),
                    IsProblematicPayment: payment.IsProblematicPayment,
                    Attempt: payment.Attempt is null
                        ? null
                        : new AttemptInCsvModel(
                            AttemptId: payment.Attempt.AttemptId,
                            IsSuccessfulAttempt: payment.Attempt.IsSuccessfulAttempt,
                            AttemptStatus: Enum.GetName(payment.Attempt.AttemptStatus),
                            PaymentMethod: Enum.GetName(payment.Attempt.PaymentMethod),
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
        string MembershipId,
        string PaymentType,
        string CurrencyCode,
        decimal Amount,
        DateTime Created,
        DateTime? Updated,
        string PaymentStatus,
        bool IsProblematicPayment,
        AttemptInCsvModel Attempt);

    internal record AttemptInCsvModel(Guid AttemptId,
        bool IsSuccessfulAttempt,
        string AttemptStatus,
        string PaymentMethod,
        string DetailsJson);

    internal sealed class PaymentCsvMapper : ClassMap<PaymentInCsvModel>
    {
        public PaymentCsvMapper()
        {
            Map(paymentModel => paymentModel.PaymentId);
            Map(paymentModel => paymentModel.MembershipId);
            Map(paymentModel => paymentModel.PaymentType);
            Map(paymentModel => paymentModel.CurrencyCode);
            Map(paymentModel => paymentModel.Amount);
            Map(paymentModel => paymentModel.Created);
            Map(paymentModel => paymentModel.Updated);
            Map(paymentModel => paymentModel.PaymentStatus);
            Map(paymentModel => paymentModel.IsProblematicPayment);

            References<AttemptCsvMapper>(attempt => attempt.Attempt);
        }
    }

    internal sealed class AttemptCsvMapper : ClassMap<AttemptInCsvModel>
    {
        public AttemptCsvMapper()
        {
            Map(attemptModel => attemptModel.IsSuccessfulAttempt);
            Map(attemptModel => attemptModel.AttemptStatus);
            Map(attemptModel => attemptModel.PaymentMethod);
            Map(attemptModel => attemptModel.DetailsJson);
        }
    }
}
