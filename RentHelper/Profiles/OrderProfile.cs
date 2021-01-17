using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RentHelper.Dtos;
using RentHelper.Models;

namespace RentHelper.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.OrderTime,
                    opt => opt.MapFrom(src => (src.OrderTime==null)? null: ((DateTime)src.OrderTime).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo) )
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.PayTime,
                    opt => opt.MapFrom(src => (src.PayTime == null)? null : ((DateTime)src.PayTime).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.ProductSend,
                    opt => opt.MapFrom(src => (src.ProductSend == null) ? null : ((DateTime)src.ProductSend).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.ProductArrive,
                    opt => opt.MapFrom(src => (src.ProductArrive == null) ? null : ((DateTime)src.ProductArrive).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.ProductSendBack,
                    opt => opt.MapFrom(src => (src.ProductSendBack == null) ? null : ((DateTime)src.ProductSendBack).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.ProductGetBack,
                    opt => opt.MapFrom(src => (src.ProductGetBack == null) ? null : ((DateTime)src.ProductGetBack).ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.Pics,
                    opt => opt.MapFrom(src => (src.Product ==null)? new List<Picture>() { new Picture() { Path = @"http://35.221.140.217/image/noPic.png" } } :src.Product.Pics)
                )
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.status,
                    opt => opt.MapFrom(src => src.status.getStatusDesc())
                )
                ;
        }
    }
}
