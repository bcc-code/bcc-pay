using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Shared.Converters;
using BccPay.Core.Shared.Helpers;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentAttemptCommand : IRequest<IStatusDetails>
    {
        public CreatePaymentAttemptCommand()
        {
        }

        public Guid PaymentId { get; set; }
        public string PaymentConfigurationId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

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
                .NotNull()
                .NotEmpty();
        }
    }

    public class CreatePaymentAttemptCommandHandler : IRequestHandler<CreatePaymentAttemptCommand, IStatusDetails>
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

        public async Task<IStatusDetails> Handle(CreatePaymentAttemptCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                        Payment.GetPaymentId(request.PaymentId), cancellationToken)
                        ?? throw new Exception("Invalid payment ID");

            var paymentConfiguration = await _documentSession.LoadAsync<PaymentConfiguration>(
                    PaymentConfiguration.GetPaymentConfigurationId(request.PaymentConfigurationId), cancellationToken)
                    ?? throw new Exception("Invalid payment configuration ID");

            if (payment.Attempts?.Where(x => x.IsActive).Any() == true)
                throw new Exception("One of the attempts is still active.");

            var (phonePrefix, phoneBody) = PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);

            var provider = _paymentProviderFactory.GetPaymentProvider(paymentConfiguration.Provider);

            var providerResult = await provider.CreatePayment(new PaymentRequestDto
            {
                Amount = decimal.Round(payment.Amount, 2, MidpointRounding.AwayFromZero),
                Address = new AddressDto
                {
                    Country = string.IsNullOrWhiteSpace(payment.CountryCode)
                        ? string.Empty
                        : AddressConverter.ConvertCountry(payment.CountryCode, CountryCodeFormat.Alpha3),
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
            }, paymentConfiguration.Settings);

            var attempt = new Attempt
            {
                PaymentAttemptId = Guid.NewGuid(),
                PaymentMethod = paymentConfiguration.Settings.PaymentMethod,
                PaymentStatus = providerResult.IsSuccessful ? AttemptStatus.WaitingForFee : AttemptStatus.Rejected,
                IsActive = providerResult.IsSuccessful,
                Created = DateTime.Now,
                StatusDetails = providerResult
            };

            payment.Updated = DateTime.Now;
            payment.AddAttempt(new List<Attempt> { attempt });
            await _documentSession.SaveChangesAsync(cancellationToken);

            return providerResult;
        }
    }
}
