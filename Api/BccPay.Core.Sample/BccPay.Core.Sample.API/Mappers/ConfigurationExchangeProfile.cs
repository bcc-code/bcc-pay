using AutoMapper;
using BccPay.Core.Cqrs.Queries;
using BccPay.Core.Sample.Contracts.Requests;

namespace BccPay.Core.Sample.API.Mappers;

public class ConfigurationExchangeProfile : Profile
{
    public ConfigurationExchangeProfile()
    {
        CreateMap<ExchangeWithConfigurationsRequestModel, GetConfigurationsWithExchangedCurrencyQuery>();
    }
}
