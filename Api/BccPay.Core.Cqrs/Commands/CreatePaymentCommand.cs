﻿using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using FluentValidation;
using MediatR;
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
            string description,
            object paymentDetails,
            string paymentType)
        {
            PayerId = payerId;
            Amount = amount;
            CurrencyCode = currencyCode;
            CountryCode = countryCode;
            Description = description;
            PaymentDetails = paymentDetails;
            PaymentType = paymentType;
        }

        public string Description { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }
        public object PaymentDetails { get; set; }
        public string PaymentType { get; set; }
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
            var payment = new Payment();

            payment.Create(request.PayerId, request.CurrencyCode, request.CountryCode, request.Amount, request.Description, (IPaymentDetails)request.PaymentDetails, request.PaymentType);

            await _documentSession.StoreAsync(payment, cancellationToken);
            await _documentSession.SaveChangesAsync(cancellationToken);

            return payment.PaymentId.ToString();
        }
    }
}
