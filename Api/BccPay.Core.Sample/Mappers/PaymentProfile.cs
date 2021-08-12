using AutoMapper;
using BccPay.Core.Contracts.Requests;
using BccPay.Core.Contracts.Responses;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Helpers;

namespace BccPay.Core.Sample.Mappers
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentRequest, CreatePaymentCommand>();

            CreateMap<CreatePaymentAttemptRequest, CreatePaymentAttemptCommand>();

            CreateMap<Payment, GetPaymentResponse>();
            CreateMap<Attempt, AttemptResponseModel>()
                .ForMember(destination
                => destination.StatusDetails, options
                    => options.MapFrom(source
                        => ReverseAbstraction<object, IStatusDetails>.GetImplementationFromAbstraction(source.StatusDetails)));

            CreateMap<MolliePaymentAttemptResponse, MollieStatusDetails>()
                .ReverseMap();
            CreateMap<NetsPaymentAttemptResponse, NetsStatusDetails>()
                .ReverseMap();

            CreateMap<IPaymentResponse, IStatusDetails>()
                .Include<MolliePaymentAttemptResponse, MollieStatusDetails>()
                .Include<NetsPaymentAttemptResponse, NetsStatusDetails>()
                .ReverseMap();
        }
    }
}
