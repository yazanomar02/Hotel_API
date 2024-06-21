﻿using Domain.Entites;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class RoomForUpdate
    {
        public int Number { get; set; }
        public int FloorNumber { get; set; }
        public Status Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
    }
}
