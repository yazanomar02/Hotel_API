using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Hotel : BaseClass
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
