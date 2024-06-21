using Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PaymentWithOut_Booking
    {
        public double TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BookingId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
