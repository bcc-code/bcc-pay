using AutoMapper;
using BccPay.Core.Contracts.Requests;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Sample.Mappers
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentRequest, CreatePaymentCommand>();

            CreateMap<CreatePaymentAttemptRequest, CreatePaymentAttemptCommand>()
                .ForMember(destination => destination.PaymentMethod, options => options.MapFrom(source => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), source.PaymentMethod, true)))
                .ForMember(destination => destination.CountryCode, options => options.MapFrom(source => source.CountryCode));
        }
    }
}
