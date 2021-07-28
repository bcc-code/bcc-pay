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
        public CreatePaymentCommand(Guid payerId,
            string currency,
            int amount,
            string country,
            PaymentMethod paymentMethod)
        {
            PayerId = payerId;
            Currency = currency;
            Amount = amount;
            Country = country;
            PaymentMethod = paymentMethod;
        }

        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
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
                // TODO: Depend on payment provider
                RuleFor(x => x.Country)
                    .MinimumLength(2)
                    .MaximumLength(3)
                    .WithMessage("Wrong country code");

                RuleFor(x => x.Currency)
                    .Must(x => IsCurrencyCodeValid(x))
                    .WithMessage("Not valid length");

                RuleFor(x => x.Amount)
                    .GreaterThan(0)
                    .NotEmpty()
                    .WithMessage("Invalid amount, must be greater than 0");

                RuleFor(x => x.Email)
                    .EmailAddress();

                RuleFor(x => x.PostalCode)
                    .Matches(new Regex(@"^([0-9]{1,9})$"));

                RuleFor(x => x.PhoneNumber)
                    .MinimumLength(10)
                    .MaximumLength(15);
                // TODO: Active payments for payer ID
            }

            /// TODO: country list check 
            /// From nets https://developers.nets.eu/nets-easy/en-EU/api/#country-codes-and-phone-prefixes

            /// TODO: currency list check
            private bool IsCurrencyCodeValid(string countryCode)
            {
                return countryCode.Length == 3;
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
                var (phonePrefix, phoneBody) = PhoneNumberConverter.GetNationalNumber(request.PhoneNumber, request.Country);

                var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

                var paymentId = await provider.CreatePayment(new PaymentRequestDto
                {
                    Amount = request.Amount,
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
