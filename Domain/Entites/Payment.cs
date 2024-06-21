using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Payment : BaseClass
    {
        public double TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public Booking Booking { get; set; }
        public int BookingId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
