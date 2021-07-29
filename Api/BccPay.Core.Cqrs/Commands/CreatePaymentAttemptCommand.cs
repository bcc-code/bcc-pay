using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Shared.Converters;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentAttemptCommand : IRequest<string>
    {
        public CreatePaymentAttemptCommand()
        {
        }

        public Guid PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string CountryCode { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
    }

    public class CreatePaymentAttemptValidator : AbstractValidator<CreatePaymentAttemptCommand>
    {
        public CreatePaymentAttemptValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address format");

            RuleFor(x => x.PhoneNumber)
                .Must(x => PhoneNumberConverter.IsPhoneNumberValid(x))
                .WithMessage("Invalid phone number");

            RuleFor(x => x.CountryCode)
                .MinimumLength(2)
                .MaximumLength(3)
                .WithMessage("Invalid country code, use alpha2, alpha3 or numeric codes");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .Matches(new Regex(@"^([0-9A-Z\s]{1,9})$"));
        }
    }

    public class CreatePaymentAttemptCommandHandler : IRequestHandler<CreatePaymentAttemptCommand, string>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public CreatePaymentAttemptCommandHandler(IPaymentProviderFactory paymentProviderFactory,
            IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
            _paymentProviderFactory = paymentProviderFactory
                ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
        }

        public async Task<string> Handle(CreatePaymentAttemptCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                        Payment.GetPaymentId(request.PaymentId), cancellationToken)
                    ?? throw new Exception();

            if (payment.PaymentStatus != PaymentStatus.Open)
                throw new Exception();

            var (phonePrefix, phoneBody) = PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);

            var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

            var checkoutPaymentId = await provider.CreatePayment(new PaymentRequestDto
            {
                Amount = decimal.Round(payment.Amount, 2, MidpointRounding.AwayFromZero),
                Address = new AddressDto
                {
                    Country = AddressConverter.ConvertCountry(request.CountryCode),
                    City = request.City,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    PostalCode = request.PostalCode
                },
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumberBody = phoneBody,
                PhoneNumberPrefix = phonePrefix,
                Currency = payment.CurrencyCode
            });

            var attempt = new Attempt
            {
                CountryCode = request.CountryCode,
                PaymentAttemptId = Guid.NewGuid(),
                PaymentIdForCheckoutForm = checkoutPaymentId,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = AttemptStatus.WaitingForFee,
                PaymentId = payment.PaymentId,
                IsActive = true,
                Created = DateTime.Now
            };

            if (payment.Attempts?.Any() == true)
                payment.CancelLastAttempt();

            payment.Updated = DateTime.Now;
            payment.AddAttempt(new List<Attempt> { attempt });
            await _documentSession.SaveChangesAsync(cancellationToken);

            return checkoutPaymentId;
        }
    }
}
