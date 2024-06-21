using Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class RoomTypeWithOut_Rooms
    {
        public string TypeName { get; set; } = string.Empty;
        public int NumOfBeds { get; set; }
        public bool IsDeleted { get; set; }
    }
}
