using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Employee : BaseClass
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public DateTime StaretdDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
