﻿using AutoMapper;
using Domain.Entites;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Profiles
{
    public class GuestProfile : Profile
    {
        public GuestProfile()
        {
            CreateMap<Guest, GuestWithOut_Booking>();
        }
    }
}
