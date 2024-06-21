using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PaymentForCreate
    {
        public double TotalAmount { get; set; }
        public int BookingId { get; set; }
    }
}
