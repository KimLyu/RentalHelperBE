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
    public class PictureProfile: Profile
    {
        public PictureProfile()
        {
            CreateMap<Picture, PictureDto>()    //從Picture映射到PictureDto
                .ForMember( //投影 將數據經過計算後 對應到目標數據中
                    dest => dest.DateTime,
                    opt => opt.MapFrom(src => src.DateTime.ToString("yyyy-MM-dd\"T\"HH:mm", DateTimeFormatInfo.InvariantInfo))
                );


        }
    }
}
