using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Room : BaseClass
    {

        public int Number { get; set; }
        public int FloorNumber { get; set; }
        public Status Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }
        public RoomType RoomType { get; set; }
        public int RoomTypeId { get; set; }
        public Booking Booking { get; set; }

    }
}
