using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Domain.Entities;
using FluentValidation;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentCommand : IRequest<string>
    {
        public CreatePaymentCommand()
        {
        }

        public CreatePaymentCommand(string payerId,
            string currencyCode,
            decimal amount,
            string countryCode,
            string description)
        {
            PayerId = payerId;
            Amount = amount;
            CurrencyCode = currencyCode;
            CountryCode = countryCode;
            Description = description;
        }

        public string Description { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .Matches(new Regex(@"^([A-Z]{3})$"))
                .NotEmpty()
                .WithMessage("Invalid currency code");

            RuleFor(x => x.CountryCode)
                .MinimumLength(2)
                .MaximumLength(3)
                .WithMessage("Invalid country code, use alpha2, alpha3 or numeric codes");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Invalid amount, must be greater than 0");
        }
    }

    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, string>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public CreatePaymentCommandHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<string> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await _documentSession.Query<PaymentsIndex.Result, PaymentsIndex>()
                        .Where(x => x.PayerId == request.PayerId
                                    && x.Amount == request.Amount
                                    && x.CurrencyCode == request.CurrencyCode
                                    && x.PaymentStatus != Enums.PaymentStatus.Canceled)
                        .OrderByDescending(x => x.CreationDate)
                        .Select(x => x.PaymentId)
                        .FirstOrDefaultAsync(cancellationToken);

            if (result != Guid.Empty)
                return result.ToString();

            var payment = new Payment();

            payment.Create(request.PayerId, request.CurrencyCode, request.CountryCode, request.Amount, request.Description);

            await _documentSession.StoreAsync(payment, cancellationToken);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return payment.PaymentId.ToString();
        }
    }
}
