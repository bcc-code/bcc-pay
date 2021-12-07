using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Shared.Converters;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Helpers;
using FluentValidation;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public record UpdatePaymentTicketCommand
        (Guid TicketId, decimal? BaseCurrencyAmount, decimal? OtherCurrencyAmount) : IRequest<PaymentTicketResponse>;

    public class UpdatePaymentTicketCommandAbstractValidator : AbstractValidator<UpdatePaymentTicketCommand>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public UpdatePaymentTicketCommandAbstractValidator(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;

            RuleFor(x => new {x.BaseCurrencyAmount, x.OtherCurrencyAmount})
                .Must(x => IsAmountsValid(x.BaseCurrencyAmount, x.OtherCurrencyAmount))
                .WithMessage("Not valid amount");

            RuleFor(x => x.TicketId)
                .MustAsync(IsTicketValidToUse)
                .WithMessage("Ticket is not valid");
        }

        private static bool IsAmountsValid(decimal? first, decimal? second)
        {
            if ((first is null or 0) && second > 0)
                return true;

            return (second is null or 0) && first > 0;
        }

        private async Task<bool> IsTicketValidToUse(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket =
                await _documentSession.LoadAsync<PaymentTicket>(PaymentTicket.GetDocumentId(ticketId),
                    cancellationToken);

            return ticket is not null && !((ticket.Updated ?? ticket.Created) < DateTime.UtcNow.AddHours(-1));
        }
    }

    internal class
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

            bool isOppositeConversion = request.BaseCurrencyAmount is 0 or null;
            var amount = isOppositeConversion
                ? request.OtherCurrencyAmount
                : request.BaseCurrencyAmount;

            if (amount is null)
                throw new InvalidOperationException();

            var providerDefinition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                PaymentProviderDefinition.GetDocumentId(ticket.PaymentDefinitionId),
                cancellationToken);

            (decimal exchangeRate, _) =
                await _currencyService.GetExchangeRateByCurrency(ticket.BaseCurrency, ticket.OtherCurrency);

            exchangeRate *= (1 + providerDefinition.Settings.Markup);

            decimal exchangeResult = isOppositeConversion
                ? Decimal.Divide(amount.Value, exchangeRate)
                : Decimal.Multiply(amount.Value, exchangeRate);

            ticket.Update(isOppositeConversion, amount.ToAmountOfDigitsAfterPoint(),
                exchangeResult.ToAmountOfDigitsAfterPoint(),
                exchangeRate);

            await _documentSession.SaveChangesAsync(cancellationToken);

            return new PaymentTicketResponse(ticket.TicketId, ticket.BaseCurrency, ticket.OtherCurrency,
                ticket.BaseCurrencyAmount.ToAmountOfDigitsAfterPoint(),
                ticket.OtherCurrencyAmount.ToAmountOfDigitsAfterPoint(),
                ticket.ExchangeRate.ToAmountOfDigitsAfterPoint(6),
                (1 / ticket.ExchangeRate).ToAmountOfDigitsAfterPoint(6), ticket.Updated, ticket.PaymentDefinitionId,
                ticket.CountryCode);
        }
    }
}