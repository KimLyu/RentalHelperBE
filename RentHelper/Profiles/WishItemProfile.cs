using AutoMapper;
using RentHelper.Dtos;
using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Profiles
{
    public class WishItemProfile : Profile
    {
        public WishItemProfile()
        {
            CreateMap<WishItem,WishItemDto>();
        }
    }
}
