using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Shared.Converters;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentCommand : IRequest<string>
    {
        public CreatePaymentCommand(string payerId,
            string currency,
            decimal amount,
            string country,
            PaymentMethod paymentMethod)
        {
            PayerId = payerId;
            Currency = currency;
            Amount = amount;
            Country = country;
            PaymentMethod = paymentMethod;
        }

        public string PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }

        public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
        {
            public CreatePaymentCommandValidator()
            {
                RuleFor(x => x.Country)
                    .MinimumLength(2)
                    .MaximumLength(3)
                    .WithMessage("Invalid country code, use alpha2, alpha3 or numeric codes");

                RuleFor(x => x.Currency)
                    .Matches(new Regex(@"^([A-Z]{3})$"))
                    .NotEmpty()
                    .WithMessage("Invalid currency code");

                RuleFor(x => x.Amount)
                    .GreaterThan(0)
                    .WithMessage("Invalid amount, must be greater than 0");

                RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.PhoneNumber)
                    .Must(x => PhoneNumberConverter.IsPhoneNumberValid(x))
                    .WithMessage("Invalid phone number");

                RuleFor(x => x.PostalCode)
                    .NotEmpty()
                    .Matches(new Regex(@"^([0-9A-Z\s]{1,9})$"));
            }
        }

        public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, string>
        {
            private readonly IAsyncDocumentSession _documentSession;
            private readonly IPaymentProviderFactory _paymentProviderFactory;

            public CreatePaymentCommandHandler(IPaymentProviderFactory paymentProviderFactory,
                IAsyncDocumentSession documentSession)
            {
                _documentSession = documentSession
                    ?? throw new ArgumentNullException(nameof(documentSession));
                _paymentProviderFactory = paymentProviderFactory
                    ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            }

            public async Task<string> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
            {
                var (phonePrefix, phoneBody) = PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);

                var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

                var paymentId = await provider.CreatePayment(new PaymentRequestDto
                {
                    Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero),
                    Address = new AddressDto
                    {
                        Country = AddressConverter.ConvertCountry(request.Country.ToUpper()),
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
                    Currency = request.Currency
                });

                var payment = new Payment();
                payment.Create(paymentId, request.PayerId, request.Currency, request.Amount, request.Country, request.PaymentMethod);

                await _documentSession.StoreAsync(payment, cancellationToken);
                await _documentSession.SaveChangesAsync(cancellationToken);

                return payment.PaymentIdForCheckoutForm;
            }
        }
    }
}
