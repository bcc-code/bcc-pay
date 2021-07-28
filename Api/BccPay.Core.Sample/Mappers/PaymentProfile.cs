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
            CreateMap<CreatePaymentRequest, CreatePaymentCommand>()
                .ForMember(destination => destination.PaymentMethod,
                    options => options.MapFrom(source => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), source.PaymentMethod, true)))
                .ForMember(destination => destination.FirstName,
                    options => options.MapFrom(source => source.Payer.FirstName))
                .ForMember(destination => destination.LastName,
                    options => options.MapFrom(source => source.Payer.LastName))
                .ForMember(destination => destination.Email,
                    options => options.MapFrom(source => source.Payer.Email))
                .ForMember(destination => destination.PhoneNumber,
                    options => options.MapFrom(source => source.Payer.PhoneNumber))
                .ForMember(destination => destination.AddressLine1,
                    options => options.MapFrom(source => source.Payer.AddressLine1))
                .ForMember(destination => destination.AddressLine2,
                    options => options.MapFrom(source => source.Payer.AddressLine2))
                .ForMember(destination => destination.City,
                    options => options.MapFrom(source => source.Payer.City))
                .ForMember(destination => destination.PostalCode,
                    options => options.MapFrom(source => source.Payer.PostalCode));
        }
    }
}
