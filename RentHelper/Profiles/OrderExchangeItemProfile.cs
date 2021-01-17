using AutoMapper;
using RentHelper.Dtos;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Profiles
{
    public class OrderExchangeItemProfile:Profile
    {
        public OrderExchangeItemProfile()
        {
            CreateMap<OrderExchangeItem, OrderExchangeItemDto>()
                .ForMember(
                    dest => dest.ExchangeItem,
                    opt => opt.MapFrom(src => src.WishItem.ExchangeItem)
                )
                ;
        }

    }
}
