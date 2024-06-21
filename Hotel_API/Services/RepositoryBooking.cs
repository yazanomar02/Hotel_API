using Data;
using Domain.Entites;
using Domain.Enum;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Hotel_API.Services
{
    public class RepositoryBooking : IRepository<Booking, BookingForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryBooking(HotelContext context)
        {
            this.context = context;
        }



        public async Task<Booking> CreateAsync(Booking booking)
        {
            if (booking == null)
            {
                Console.WriteLine("Booking is null");
                return null;
            }

            // التحقق من وجود الموظف الذي يقوم بعملية الحجز
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == booking.EmployeeId && !e.IsDeleted);

            if (employee == null)
            {
                await Console.Out.WriteLineAsync("Employee not found or deleted");
                return null;
            }

            // التحقق من وجود غرفة متاحة
            var room = await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == booking.RoomId && r.Status == Status.ready);

            if (room == null)
            {
                await Console.Out.WriteLineAsync("No room available");
                return null;
            }

            // إنشاء كائن للحجز الجديد
            var newBooking = new Booking
            {
                CheckinAt = booking.CheckinAt,
                CheckoutAt = booking.CheckoutAt,
                Price = booking.Price,
                //Guest = guest,
                Room = room,
                Employee = employee
            };

            room.Status = Status.occupied; // تغيير حالة الغرفة

            await context.Bookings.AddAsync(newBooking); // إضافة الحجز لقاعدة البيانات

            context.Rooms.Update(room); // تحديث معلومات الغرفة
            await context.SaveChangesAsync();

            return newBooking;
        }




        public async Task<Booking> UpdateAsync(Booking booking)
        {
            // التحقق من وجود الحجز في قاعدة البيانات
            var existingBooking = await context.Bookings
                                .FirstOrDefaultAsync(b => b.Id == booking.Id && !b.IsDeleted);

            if (existingBooking == null)
            {
                await Console.Out.WriteLineAsync("Booking not found!!");
                return null;
            }

            // التحقق من وجود الموظف الجديد الذي يقوم بعملية الحجز
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == booking.EmployeeId && !e.IsDeleted);

            if (employee == null)
            {
                Console.WriteLine("Modified employee is not available");
                return null;
            }

            // التحقق من حالة الغرفة الجديدة
            var newRoom = await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == booking.RoomId);

            if (newRoom == null || newRoom.Status != Status.ready)
            {
                Console.WriteLine("Modified room not available");
                return null;
            }

            // تحديث حالة الغرفة إذا كانت قد تغيرت
            if (existingBooking.RoomId != booking.RoomId)
            {
                // تعيين حالة الغرفة القديمة إلى جاهزة
                var oldRoom = await context.Rooms
                    .FirstOrDefaultAsync(r => r.Id == existingBooking.RoomId);

                if (oldRoom != null)
                {
                    oldRoom.Status = Status.ready;
                    context.Rooms.Update(oldRoom);
                }

                // تعيين حالة الغرفة الجديدة إلى مشغولة
                newRoom.Status = Status.occupied;
                context.Rooms.Update(newRoom);
            }

            // تحديث الحجز الموجود
            existingBooking.CheckinAt = booking.CheckinAt;
            existingBooking.CheckoutAt = booking.CheckoutAt;
            existingBooking.Price = booking.Price;
            existingBooking.IsDeleted = booking.IsDeleted;
            existingBooking.EmployeeId = booking.EmployeeId;
            existingBooking.RoomId = booking.RoomId;

            context.Bookings.Update(existingBooking);
            await context.SaveChangesAsync();

            return existingBooking;
        }



        public async Task<Booking> PartiallyUpdateAsync(int bookingId, JsonPatchDocument<BookingForUpdate> patchDocument)
        {
            var existingBooking = await context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (existingBooking == null)
                return null;

            var bookingToPatch = new BookingForUpdate
            {
                CheckinAt = existingBooking.CheckinAt,  
                CheckoutAt = existingBooking.CheckoutAt,
                Price = existingBooking.Price,
                IsDeleted = existingBooking.IsDeleted,
                //GuestId = existingBooking.GId,
                EmployeeId = existingBooking.EmployeeId,
                RoomId = existingBooking.RoomId,
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(bookingToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }

            var checkEmployee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == bookingToPatch.EmployeeId && !e.IsDeleted);

            if (checkEmployee == null)
            {
                await Console.Out.WriteLineAsync("The new employee is not available");
                return null;
            }

            var checkRoom = await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == bookingToPatch.RoomId && !r.IsDeleted);

            if (checkRoom == null)
            {
                await Console.Out.WriteLineAsync("The new room is not available");
                return null;
            }

            // تحديث حالة الغرفة إذا كانت قد تغيرت
            if (existingBooking.RoomId != bookingToPatch.RoomId)
            {
                // تعيين حالة الغرفة القديمة إلى جاهزة
                var oldRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Id == existingBooking.RoomId);
                if (oldRoom != null)
                {
                    oldRoom.Status = Status.ready;
                    context.Rooms.Update(oldRoom);
                }

                // تعيين حالة الغرفة الجديدة إلى مشغولة
                checkRoom.Status = Status.occupied;
                context.Rooms.Update(checkRoom);
            }

            existingBooking.CheckinAt = bookingToPatch.CheckinAt;
            existingBooking.CheckoutAt = bookingToPatch.CheckoutAt;
            existingBooking.Price = bookingToPatch.Price;
            existingBooking.IsDeleted = bookingToPatch.IsDeleted;
            existingBooking.RoomId = bookingToPatch.RoomId;
            existingBooking.EmployeeId = bookingToPatch.EmployeeId;


            await context.SaveChangesAsync();

            return existingBooking;
        }



        public async Task<Booking> DeleteAsync(int bookingId)
        {
            var existingBooking = await context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);

            if (existingBooking == null)
                return null;

            existingBooking.IsDeleted = true;

            context.Bookings.Update(existingBooking);
            await context.SaveChangesAsync();

            return existingBooking;
        }



        public async Task<(List<Booking>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, string keyword = null)
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Bookings.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var bookings = await context.Bookings.Skip(pageSize * (pageNumber - 1))
                                        .Take(pageSize)
                                        .ToListAsync();

            return (bookings, paginationData);
        }



        public async Task<Booking> GetByIdAsync(int bookingId)
        {
            var booking = await context.Bookings
                        .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return null;

            return booking;
        }
    }
}
