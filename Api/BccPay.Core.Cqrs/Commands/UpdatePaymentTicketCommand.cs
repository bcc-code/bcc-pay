using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Sample.Contracts.Responses;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public record UpdatePaymentTicketCommand
        (Guid TicketId, decimal BaseCurrencyAmount, decimal OtherCurrencyAmount) : IRequest<PaymentTicketResponse>;

    public class UpdatePaymentTicketCommandAbstractValidator : AbstractValidator<UpdatePaymentTicketCommand>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public UpdatePaymentTicketCommandAbstractValidator(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;

            RuleFor(x => new {x.BaseCurrencyAmount, x.OtherCurrencyAmount})
                .Must(x => x.BaseCurrencyAmount > 0 || x.OtherCurrencyAmount > 0)
                .WithMessage("The amount must be greater than 0");

            RuleFor(x => x.TicketId)
                .MustAsync(
                    async (ticketId, cancellationToken) => await IsTicketValidToUse(ticketId, cancellationToken))
                .WithMessage("Ticket is not valid");
        }

        private async Task<bool> IsTicketValidToUse(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket =
                await _documentSession.LoadAsync<PaymentTicket>(PaymentTicket.GetDocumentId(ticketId),
                    cancellationToken);

            return ticket is not null && !(ticket.Updated < DateTime.UtcNow.AddHours(-1));
        }
    }

    public class
        UpdatePaymentTicketCommandHandler : IRequestHandler<UpdatePaymentTicketCommand, PaymentTicketResponse>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ICurrencyService _currencyService;

        public UpdatePaymentTicketCommandHandler(IAsyncDocumentSession documentSession,
            ICurrencyService currencyService)
        {
            _documentSession = documentSession;
            _currencyService = currencyService;
        }

        public async Task<PaymentTicketResponse> Handle(UpdatePaymentTicketCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _documentSession.LoadAsync<PaymentTicket>(PaymentTicket.GetDocumentId(request.TicketId),
                cancellationToken);

            bool fromOpposite = request.BaseCurrencyAmount is not 0;

            Currencies fromCurrency;
            Currencies toCurrency;

            if (fromOpposite)
            {
                toCurrency = ticket.DefinedCurrency;
                fromCurrency = ticket.BaseCurrency;
            }
            else
            {
                fromCurrency = ticket.DefinedCurrency;
                toCurrency = ticket.BaseCurrency;
            }

            var providerDefinition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                PaymentProviderDefinition.GetDocumentId(ticket.PaymentDefinitionId),
                cancellationToken);

            (decimal exchangeRate, bool _, DateTime? _) =
                await _currencyService.GetExchangeRateByCurrency(fromCurrency, toCurrency);

            exchangeRate *= (1 + providerDefinition.Settings.Markup);

            (decimal input, decimal output) =
                ExchangeAndNormalize(fromOpposite, request.BaseCurrencyAmount, request.OtherCurrencyAmount,
                    exchangeRate);

            ticket.Update(input, output, exchangeRate);

            await _documentSession.SaveChangesAsync(cancellationToken);

            return new PaymentTicketResponse(input, output, exchangeRate);
        }

        private static (decimal, decimal) ExchangeAndNormalize(bool fromOpposite, decimal baseAmount,
            decimal otherAmount, decimal rate)
        {
            decimal baseResult;
            decimal otherResult;

            if (fromOpposite)
            {
                baseResult = baseAmount;
                otherResult = baseAmount * rate;
            }
            else
            {
                baseResult = otherAmount * rate;
                otherResult = otherAmount;
            }

            return (baseResult, otherResult);
        }
    }
}