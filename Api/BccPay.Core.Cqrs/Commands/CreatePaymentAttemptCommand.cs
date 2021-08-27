using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Exceptions;
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
        public string AcceptLanguage { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }

        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public bool IsMobile { get; set; }
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
        private readonly IMediator _mediator;

        public CreatePaymentAttemptCommandHandler(
            IPaymentProviderFactory paymentProviderFactory,
            IAsyncDocumentSession documentSession,
            IMediator mediator)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
            _paymentProviderFactory = paymentProviderFactory
                ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            _mediator = mediator;
        }

        public async Task<IStatusDetails> Handle(CreatePaymentAttemptCommand request, CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                        Payment.GetDocumentId(request.PaymentId), cancellationToken)
                    ?? throw new NotFoundException("Invalid payment ID");

            if (payment.PaymentStatus == PaymentStatus.Closed 
                || payment.PaymentStatus == PaymentStatus.Paid
                || payment.PaymentStatus == PaymentStatus.Refunded)
                throw new UpdatePaymentAttemptForbiddenException("Payment is completed.");

            var countryCode = request.CountryCode ?? payment.CountryCode;

            var paymentConfiguration = await _documentSession.LoadAsync<PaymentConfiguration>(
                    PaymentConfiguration.GetDocumentId(request.PaymentConfigurationId), cancellationToken)
                    ?? throw new Exception("Invalid payment configuration ID");

            var countryAvailableConfigurations = await _mediator.Send(new GetCountryPaymentConfigurationsQuery(countryCode), cancellationToken);

            var provider = _paymentProviderFactory.GetPaymentProvider(paymentConfiguration.Provider);

            if (payment.Attempts?.Count >= 1)
            {
                var attempt = payment.Attempts.Last();

                var paymentProvider = _paymentProviderFactory.GetPaymentProvider(attempt.PaymentProvider);
                if (await paymentProvider.TryCancelPreviousPaymentAttempt(attempt) == AttemptCancellationResult.AlreadyCompleted)
                {
                    payment.UpdateAttempt(attempt);
                    await _documentSession.SaveChangesAsync(cancellationToken);
                    throw new UpdatePaymentAttemptForbiddenException("Attempt is completed.");
                }
            }

            if (!countryAvailableConfigurations.Any(x => x.Id == request.PaymentConfigurationId))
                throw new InvalidPaymentException($"The payment configuration {request.PaymentConfigurationId} is not available for the country '{countryCode}'");

            var (phonePrefix, phoneBody) = PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);

            var paymentRequest = new PaymentRequestDto
            {
                PaymentId = payment.PaymentId.ToString(),
                Amount = decimal.Round(payment.Amount, 2, MidpointRounding.AwayFromZero),
                Address = new AddressDto
                {
                    Country = string.IsNullOrWhiteSpace(countryCode)
                        ? string.Empty
                        : AddressConverter.ConvertCountry(countryCode, CountryCodeFormat.Alpha3),
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
                Currency = payment.CurrencyCode,
                NotificationAccessToken = Guid.NewGuid().ToString(),
                AcceptLanguage = request.AcceptLanguage,
                Description = payment.Description,
                IsMobile = request.IsMobile
            };

            var providerResult = await provider.CreatePayment(paymentRequest, paymentConfiguration.Settings);

            var attemptToAdd = new Attempt
            {
                PaymentAttemptId = Guid.NewGuid(),
                PaymentMethod = paymentConfiguration.Settings.PaymentMethod,
                AttemptStatus = providerResult.IsSuccess ? AttemptStatus.Processing : AttemptStatus.Failed,
                Created = DateTime.UtcNow,
                StatusDetails = providerResult,
                CountryCode = countryCode,
                NotificationAccessToken = paymentRequest.NotificationAccessToken,
                PaymentProvider = paymentConfiguration.Provider
            };

            payment.Updated = DateTime.UtcNow;
            payment.AddAttempt(new List<Attempt> { attemptToAdd });
            await _documentSession.SaveChangesAsync(cancellationToken);

            return providerResult;
        }
    }
}
