using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Shared.Converters;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentAttemptCommand : IRequest<IStatusDetails>
    {
        public Guid PaymentId { get; set; }
        public string ProviderDefinitionId { get; set; }
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
        public bool IsHostedCheckout { get; set; }
        public Guid? TicketId { get; set; }
        public bool? UsePaymentIdAsRouteInRedirectUrl { get; set; }
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

    internal class CreatePaymentAttemptCommandHandler : IRequestHandler<CreatePaymentAttemptCommand, IStatusDetails>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly IMediator _mediator;
        private readonly InternalSettings _internalSettings;

        public CreatePaymentAttemptCommandHandler(
            IPaymentProviderFactory paymentProviderFactory,
            IAsyncDocumentSession documentSession,
            IMediator mediator, InternalSettings internalSettings)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
            _paymentProviderFactory = paymentProviderFactory
                                      ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
            _mediator = mediator
                        ?? throw new ArgumentNullException(nameof(mediator));
            _internalSettings = internalSettings
                                ?? throw new ArgumentNullException(nameof(internalSettings));
        }

        public async Task<IStatusDetails> Handle(CreatePaymentAttemptCommand request,
            CancellationToken cancellationToken)
        {
            var payment = await _documentSession.LoadAsync<Payment>(
                              Payment.GetDocumentId(request.PaymentId), cancellationToken)
                          ?? throw new NotFoundException("Invalid payment ID");

            if (payment.PaymentStatus is PaymentStatus.Closed or PaymentStatus.Paid or PaymentStatus.Refunded)
                throw new UpdatePaymentAttemptForbiddenException("Payment is completed.");

            PaymentProviderDefinition paymentProviderDefinition = null;
            PaymentTicket ticket = null;

            if (request.TicketId is not null)
            {
                ticket = await _documentSession.LoadAsync<PaymentTicket>(
                    PaymentTicket.GetDocumentId(request.TicketId.Value), cancellationToken);

                paymentProviderDefinition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                    PaymentProviderDefinition.GetDocumentId(ticket.PaymentDefinitionId),
                    cancellationToken);
            }
            else
            {
                paymentProviderDefinition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                                                PaymentProviderDefinition.GetDocumentId(request.ProviderDefinitionId),
                                                cancellationToken)
                                            ?? throw new Exception("Invalid payment configuration ID");
            }

            var countryCode = AddressConverter.ConvertCountry(ticket is not null
                ? ticket.CountryCode
                : request.CountryCode ?? payment.CountryCode, _internalSettings.StoreCountryCodeFormat);

            var countryAvailableConfigurations =
                await _mediator.Send(new GetPaymentConfigurationsByQuery(countryCode), cancellationToken);

            var provider = _paymentProviderFactory.GetPaymentProvider(paymentProviderDefinition.Provider);

            if (payment.Attempts?.Count >= 1)
            {
                var attempt = payment.Attempts.Last();

                var paymentProvider = _paymentProviderFactory.GetPaymentProvider(attempt.PaymentProvider);
                if (await paymentProvider.TryCancelPreviousPaymentAttempt(attempt) ==
                    AttemptCancellationResult.AlreadyCompleted)
                {
                    payment.UpdateAttempt(attempt);
                    await _documentSession.SaveChangesAsync(cancellationToken);
                    throw new UpdatePaymentAttemptForbiddenException("Attempt is completed.");
                }
            }

            if (!countryAvailableConfigurations.PaymentConfigurations.Any(x =>
                x.PaymentProviderDefinitionIds.Contains(paymentProviderDefinition.PaymentDefinitionCode)))
                throw new InvalidPaymentException(
                    $"The payment configuration {request.ProviderDefinitionId} is not available for the country '{countryCode}'");

            (string phonePrefix, string phoneBody) =
                PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);


            var attemptToAdd = new Attempt
            {
                PaymentAttemptId = Guid.NewGuid(),
                PaymentMethod = paymentProviderDefinition.Settings.PaymentMethod,
                Created = DateTime.UtcNow,
                CountryCode = countryCode,
                PaymentProvider = paymentProviderDefinition.Provider,
                ProviderDefinitionId = request.ProviderDefinitionId
            };

            var paymentRequest = new PaymentRequestDto
            {
                PaymentId = payment.PaymentId.ToString(),
                Amount = decimal.Round(payment.Amount, 2, MidpointRounding.AwayFromZero),
                Address = new AddressDto
                {
                    Country = string.IsNullOrWhiteSpace(countryCode)
                        ? string.Empty
                        : AddressConverter.ConvertCountry(countryCode),
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
                BaseCurrency = payment.CurrencyCode,
                OtherCurrency = paymentProviderDefinition.Settings.Currency.ToString(),
                NotificationAccessToken = Guid.NewGuid().ToString(),
                AcceptLanguage = request.AcceptLanguage,
                Description = payment.Description,
                IsHostedCheckout = request.IsHostedCheckout,
                Ticket = ticket,
                UsePaymentIdAsRouteInRedirectUrl = request.UsePaymentIdAsRouteInRedirectUrl,
                AttemptId = attemptToAdd.PaymentAttemptId.ToString()
            };

            var providerResult = await provider.CreatePayment(paymentRequest, paymentProviderDefinition.Settings);

            attemptToAdd.StatusDetails = providerResult;
            attemptToAdd.AttemptStatus = providerResult.IsSuccess ? AttemptStatus.Processing : AttemptStatus.Failed;
            attemptToAdd.NotificationAccessToken = paymentRequest.NotificationAccessToken;

            payment.Updated = DateTime.UtcNow;
            payment.AddAttempt(new List<Attempt> { attemptToAdd });
            await _documentSession.SaveChangesAsync(cancellationToken);

            return providerResult;
        }
    }
}