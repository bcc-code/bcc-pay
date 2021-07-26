using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentCommand : IRequest<string>
    {
        public CreatePaymentCommand(Guid payerId,
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

        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string Email { get; set; }
        public string PhoneNumberPrefix { get; set; }
        public string PhoneNumberBody { get; set; }
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
                RuleFor(x => new { x.Country, x.Currency })
                    .Must(x => IsCountryCodeValid(x.Country) && IsCurrencyCodeValid(x.Currency))
                    .WithMessage("Not valid length");

                RuleFor(x => x.Amount)
                    .GreaterThan(0)
                    .NotEmpty()
                    .WithMessage("Invalid amount, must be greater than 0");

                RuleFor(x => x.Email)
                    .EmailAddress();

                RuleFor(x => x.PhoneNumberPrefix)
                    .Matches(new Regex(@"^\(?(\+[0-9]{2})|(\+[0-9]{3})\)"));

                RuleFor(x => x.PostalCode)
                    .Matches(new Regex(@"^\(?([0-9])"));

                RuleFor(x => x.PhoneNumberBody)
                    .Matches(new Regex(@"^\(?([0-9])"));
                // TODO: Active payments for payer ID
            }

            /// TODO: country list check 
            /// From nets https://developers.nets.eu/nets-easy/en-EU/api/#country-codes-and-phone-prefixes
            private bool IsCountryCodeValid(string countryCode)
            {
                return countryCode.Length == 3;
            }

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
                try
                {
                    var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

                    var paymentId = await provider.CreatePayment(new PaymentRequestDto
                    {
                        Amount = request.Amount,
                        Address = new AddressDto
                        {
                            Country = request.Country,
                            City = request.City,
                            AddressLine1 = request.AddressLine1,
                            AddressLine2 = request.AddressLine2,
                            PostalCode = request.PostalCode
                        },
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumberBody = request.PhoneNumberBody,
                        PhoneNumberPrefix = request.PhoneNumberPrefix,
                        Currency = request.Currency
                    });

                    var payment = new Payment();
                    payment.Create(paymentId, request.PayerId, request.Currency, request.Amount, request.Country, request.PaymentMethod);

                    await _documentSession.StoreAsync(payment, cancellationToken);
                    await _documentSession.SaveChangesAsync(cancellationToken);

                    return payment.PaymentIdForCheckoutForm;
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                    throw;
                }
            }
        }
    }
}
