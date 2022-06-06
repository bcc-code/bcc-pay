using System.Linq;
using AutoMapper;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Sample.Contracts.Requests;
using BccPay.Core.Sample.Contracts.Responses;

namespace BccPay.Core.Sample.Mappers;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<CreatePaymentRequest, CreatePaymentCommand>();

        CreateMap<CreatePaymentAttemptRequest, CreatePaymentAttemptCommand>();

        CreateMap<Payment, GetPaymentResponse>()
            .ForMember(destination
                => destination.PaymentDetails, options
                => options.MapFrom(source
                    => source.PaymentDetails));

        CreateMap<Attempt, AttemptResponseModel>()
            .ForMember(destination
                => destination.StatusDetails, options
                => options.MapFrom(source
                    => ReverseAbstraction<object, IStatusDetails>.GetImplementationFromAbstraction(
                        source.StatusDetails)));

        CreateMap<MollieStatusDetails, MolliePaymentAttemptResponse>()
            .ReverseMap();
        CreateMap<NetsStatusDetails, NetsPaymentAttemptResponse>()
            .ReverseMap();

        CreateMap<IStatusDetails, IPaymentAttemptResponse>()
            .Include<MollieStatusDetails, MolliePaymentAttemptResponse>()
            .Include<NetsStatusDetails, NetsPaymentAttemptResponse>()
            .ForMember(destination => destination.Error, options => options.MapFrom(
                source => source.Errors != null ? source.Errors.First() : null))
            .ReverseMap();
    }
}
