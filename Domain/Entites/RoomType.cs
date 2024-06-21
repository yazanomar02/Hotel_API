using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class RoomType : BaseClass
    {
        public string TypeName { get; set; } = string.Empty;
        public int NumOfBeds { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
