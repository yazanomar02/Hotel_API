using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Booking : BaseClass
    {
        public DateTime CheckinAt { get; set; }
        public DateTime CheckoutAt { get; set; }
        public double Price { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guest Guest { get; set; }
        public Room Room { get; set; }
        public int RoomId { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();

    }
}
