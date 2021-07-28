﻿using BccPay.Core.Domain.Entities;
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
            decimal amount)
        {
            PayerId = payerId;
            Currency = currency;
            Amount = amount;
        }

        public string PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
        {
            public CreatePaymentCommandValidator()
            {
                RuleFor(x => x.Currency)
                    .Matches(new Regex(@"^([A-Z]{3})$"))
                    .NotEmpty()
                    .WithMessage("Invalid currency code");

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
                var paymentId = Guid.NewGuid();

                payment.Create(paymentId, request.PayerId, request.Currency, request.Amount);

                await _documentSession.StoreAsync(payment, cancellationToken);
                await _documentSession.SaveChangesAsync(cancellationToken);

                return paymentId.ToString();
            }
        }
    }
}
