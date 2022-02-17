using AutoMapper;
using Business.DTOs.OfferDTOs;
using Business.DTOs.ProductDTOs;
using Business.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mapper.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, GetProductDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.UsageStatus, opt => opt.MapFrom(src => src.UsageStatus.Name))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color.Name))
                .ForMember(dest => dest.IsOfferable, opt => opt.MapFrom(src => src.IsOfferable == false
                         ? "Teklife Kapalı"
                         : "Teklife Açık"))
                .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => src.IsSold == false
                         ? "Satışta!"
                         : "Satıldı!"));
            CreateMap<Offer, GetOfferDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ConfirmStatus, opt => opt.MapFrom(src =>Enum.GetName(typeof(ConfirmStatus),src.ConfirmStatus)));
        }
    }
}
