using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Shared.Constants;
using FluentValidation;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Commands
{
    public record CreatePaymentTicketCommand(string PaymentDefinitionId, Currencies BaseCurrency, string PayerId,
        string CountryCode) : IRequest<Guid>;

    public class CreatePaymentTicketCommandAbstractValidator : AbstractValidator<CreatePaymentTicketCommand>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public CreatePaymentTicketCommandAbstractValidator(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));

            RuleFor(x => x.CountryCode)
                .MinimumLength(2)
                .WithMessage("Invalid country code, use alpha2, alpha3 or numeric codes");

            RuleFor(x => new { x.PaymentDefinitionId, x.CountryCode })
                .MustAsync(
                    async (condition, cancellationToken) => await IsDefinitionValid(condition.PaymentDefinitionId,
                        condition.CountryCode, cancellationToken))
                .WithMessage("Definition not found, either not available in your country");

            RuleFor(x => x.PayerId)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Incorrect payer id");
        }

        private async Task<bool> IsDefinitionValid(string definitionId, string countryCode,
            CancellationToken cancellationToken)
        {
            var definition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                PaymentProviderDefinition.GetDocumentId(definitionId), cancellationToken);

            if (definition is null)
                return false;

            if (countryCode == BccPayConstants.Default)
                return true;

            var availableConfigurations = await _documentSession
                .Query<PaymentConfiguration>()
                .Where(x => x.PaymentProviderDefinitionIds.Contains(definitionId))
                .ToListAsync(cancellationToken);

            return availableConfigurations.Where(x => x.CountryCode == BccPayConstants.Default || x.CountryCode == countryCode).Any();
        }
    }

    public class
        CreatePaymentTicketCommandHandler : IRequestHandler<CreatePaymentTicketCommand,
            Guid>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ICurrencyService _currencyService;

        public CreatePaymentTicketCommandHandler(IAsyncDocumentSession documentSession,
            ICurrencyService currencyService)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
            _currencyService = currencyService
                               ?? throw new ArgumentNullException(nameof(currencyService));
        }

        public async Task<Guid> Handle(CreatePaymentTicketCommand request,
            CancellationToken cancellationToken)
        {
            var definition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                PaymentProviderDefinition.GetDocumentId(request.PaymentDefinitionId), cancellationToken);

            (decimal exchangeRate, _) =
                await _currencyService.GetExchangeRateByCurrency(request.BaseCurrency,
                    definition.Settings.Currency);

            exchangeRate *= (1 + definition.Settings.Markup);

            var ticket = new PaymentTicket();

            ticket.Create(request.BaseCurrency,
                definition.Settings.Currency,
                request.PaymentDefinitionId,
                request.PayerId,
                request.CountryCode,
                exchangeRate);

            await _documentSession.StoreAsync(ticket, cancellationToken);

            await _documentSession.SaveChangesAsync(cancellationToken);

            return ticket.TicketId;
        }
    }
}