using AutoMapper;
using RentHelper.Dtos;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.createTime,
                    opt => opt.MapFrom(src => (src.createTime == null) ? null : ((DateTime)src.createTime).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                ;
        }
        
    }
}
