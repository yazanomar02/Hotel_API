using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class BookingForUpdate
    {
        public DateTime CheckinAt { get; set; }
        public DateTime CheckoutAt { get; set; }
        public double Price { get; set; }
        //public int GuestId { get; set; }
        public int RoomId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDeleted { get; set; } = false; 
    }
}
