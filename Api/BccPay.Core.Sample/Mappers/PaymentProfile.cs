using AutoMapper;
using BccPay.Core.Contracts.Requests;
using BccPay.Core.Contracts.Responses;
using BccPay.Core.Cqrs.Commands;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using Newtonsoft.Json;
using System;

namespace BccPay.Core.Sample.Mappers
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentRequest, CreatePaymentCommand>();

            CreateMap<CreatePaymentAttemptRequest, CreatePaymentAttemptCommand>()
                .ForMember(destination
                => destination.PaymentMethod, options
                    => options.MapFrom(source
                        => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), source.PaymentMethod, true)));

            CreateMap<Payment, GetPaymentResponse>();
            CreateMap<Attempt, AttemptResponseModel>()
                .ForMember(destination
                => destination.StatusDetails, options
                    => options.MapFrom(source
                        => JsonConvert.DeserializeObject<object>(
                            JsonConvert.SerializeObject(source.StatusDetails, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
                           new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
        }
    }
}
