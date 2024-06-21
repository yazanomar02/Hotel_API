
using Data;
using Domain.Entites;
using Domain.Enum;

namespace Hotel_Management
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //    using (HotelContext context = new HotelContext())
            //    {
            //        context.Database.EnsureCreated();
            //    }



            //    RepositoryHotel repoHotel = new RepositoryHotel();
            //    var hotel = new Hotel()
            //    {
            //        Name = "Tranquil Haven Hotel",
            //        Email = "TranquilHaven@gmail.com",
            //        Phone = "0618963547",
            //        Address = "Mountains Alalb",
            //    };
            //    // إضافة فندق
            //    repoHotel.Create(hotel);



            //    RepositoryEmployee repoEmployee = new RepositoryEmployee();
            //    var employee = new Employee()
            //    {
            //        FirstName = "Yazan",
            //        LastName = "Omar",
            //        Title = "Syria, Aleppo, Aazaz",
            //        DOB = new DateTime(2002, 5, 13),
            //        Email = "yazan@gmail.com",
            //        StaretdDate = new DateTime(2024, 1, 1),
            //        Hotel = hotel
            //    };
            //    // إضافة موظف
            //    repoEmployee.Create(employee);



            //    RepositoryRoom repoRoom = new RepositoryRoom();
            //    var type1 = new RoomType()
            //    {
            //        TypeName = "Single Room",
            //        NumOfBeds = 1,
            //    };

            //    var type2 = new RoomType()
            //    {
            //        TypeName = "Double Room",
            //        NumOfBeds = 2,
            //    };

            //    var type3 = new RoomType()
            //    {
            //        TypeName = "Deluxe Room",
            //        NumOfBeds = 1,
            //    };

            //    var type4 = new RoomType()
            //    {
            //        TypeName = "Family Room",
            //        NumOfBeds = 4,
            //    };



            //    var room1 = new Room()
            //    {
            //        Number = 1,
            //        FloorNumber = 1,
            //        Status = Status.ready,
            //        RoomType = type1,
            //        Hotel = hotel
            //    };
            //    var room2 = new Room()
            //    {
            //        Number = 2,
            //        FloorNumber = 2,
            //        Status = Status.dirty,
            //        RoomType = type2,
            //        Hotel = hotel
            //    };
            //    var room3 = new Room()
            //    {
            //        Number = 3,
            //        FloorNumber = 3,
            //        Status = Status.ready,
            //        RoomType = type3,
            //        Hotel = hotel
            //    };
            //    var room4 = new Room()
            //    {
            //        Number = 4,
            //        FloorNumber = 3,
            //        Status = Status.out_of_order,
            //        RoomType = type4,
            //        Hotel = hotel
            //    };

            //    // إضافة أربع غرف مع أنواعهم
            //    repoRoom.Create(room1);
            //    repoRoom.Create(room2);
            //    repoRoom.Create(room3);
            //    repoRoom.Create(room4);



            //    RepositoryGuest repoGuest = new RepositoryGuest();
            //    var guest = new Guest()
            //    {
            //        FirstName = "Osama",
            //        LastName = "Omar",
            //        DOB = new DateTime(1976, 5, 10),
            //        Email = "osama@gmail.com",
            //        Phone = "0648523697",
            //    };
            //    // إضافة ضيف
            //    repoGuest.Create(guest);
            //    // حجز للضيف (بدون دفع
            //    repoGuest.BookingToGuest(employee.Id, guest.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 99.5);
            //    // الدفع لحجز الضيف
            //    repoGuest.AddPaymentToBooking(1, 50);


            //    // الاستعلام
            //    repoHotel.GetAll();
            //    repoEmployee.GetAll();
            //    repoRoom.GetAll();
            //    repoGuest.GetAll();
            //
        }
    }
}
