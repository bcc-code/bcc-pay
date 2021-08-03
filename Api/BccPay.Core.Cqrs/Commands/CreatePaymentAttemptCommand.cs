﻿using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders;
using BccPay.Core.Shared.Converters;
using BccPay.Core.Shared.Helpers;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Cqrs.Commands
{
    public class CreatePaymentAttemptCommand : IRequest<IStatusDetails>
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

            if (payment.Attempts?.Where(x => x.IsActive).Any() == true)
                throw new Exception("One of the attempts is still active.");

            var (phonePrefix, phoneBody) = PhoneNumberConverter.ParseToNationalNumberAndPrefix(request.PhoneNumber);

            var provider = _paymentProviderFactory.GetPaymentProvider(request.PaymentMethod.ToString());

            var paymentRequest = new PaymentRequestDto
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
                Currency = payment.CurrencyCode,
                NotificationAccessToken = Guid.NewGuid().ToString()
            };

            var providerResult = await provider.CreatePayment(paymentRequest);

            var attempt = new Attempt
            {
                PaymentAttemptId = Guid.NewGuid(),
                PaymentMethod = request.PaymentMethod,
                AttemptStatus = providerResult.IsSuccessful ? AttemptStatus.WaitingForCharge : AttemptStatus.RejectedEitherCancelled,
                IsActive = providerResult.IsSuccessful,
                Created = DateTime.Now,
                StatusDetails = providerResult,
                NotificationAccessToken = paymentRequest.NotificationAccessToken
            };

            payment.Updated = DateTime.Now;
            payment.AddAttempt(new List<Attempt> { attempt });
            await _documentSession.SaveChangesAsync(cancellationToken);

            return providerResult;
        }
    }
}
