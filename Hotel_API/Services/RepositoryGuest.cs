using Data;
using Domain.Entites;
using Domain.Enum;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Management
{
    public class RepositoryGuest : IRepository<Guest, GuestForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryGuest(HotelContext context)
        {
            this.context = context;
        }



        public async Task<Guest> CreateAsync(Guest guest)
        {
            if (guest != null)
            {
                var exsitingBooking = await context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == guest.BookingId && !b.IsDeleted);

                if(exsitingBooking == null) 
                {
                    await Console.Out.WriteLineAsync("Booking not found!!");
                    return null;
                }

                context.Guests.Add(guest);
                await context.SaveChangesAsync();

                await Console.Out.WriteLineAsync("Guest added successfully.");

                return guest;
            }
            return null;
        }


        public async Task<Guest> UpdateAsync(Guest guest)
        {
            // التحقق من وجود الضيف في قاعدة البيانات
            var exsitingGuest = await context.Guests
                                .FirstOrDefaultAsync(g => g.Id == guest.Id && !g.IsDeleted);

            var exsitingBooking = await context.Bookings
                                .FirstOrDefaultAsync(b => b.Id == guest.BookingId && !b.IsDeleted);

            if (exsitingBooking == null)
            {
                await Console.Out.WriteLineAsync(" New booking not found!!");
                return null;
            }

            if (exsitingGuest != null)
            {
                // تحديث المعلومات
                exsitingGuest.FirstName = guest.FirstName;
                exsitingGuest.LastName = guest.LastName;
                exsitingGuest.Phone = guest.Phone;
                exsitingGuest.Email = guest.Email;
                exsitingGuest.DOB = guest.DOB;
                exsitingGuest.IsDeleted = guest.IsDeleted;
                exsitingGuest.BookingId = guest.BookingId;

                context.Guests.Update(exsitingGuest);
                await context.SaveChangesAsync();
            }

            await Console.Out.WriteLineAsync("Guest not found!!");
            return exsitingGuest;
        }



        public async Task<Guest> PartiallyUpdateAsync(
            int guestId, JsonPatchDocument<GuestForUpdate> patchDocument
            )
        {
            var existingGuest = await context.Guests.FirstOrDefaultAsync(g => g.Id == guestId);

            if (existingGuest == null)
                return null;

            var guestToPatch = new GuestForUpdate
            {
                FirstName = existingGuest.FirstName,
                LastName = existingGuest.LastName,
                Email = existingGuest.Email,
                Phone = existingGuest.Phone,
                DOB = existingGuest.DOB,
                IsDeleted = existingGuest.IsDeleted,
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(guestToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }

            existingGuest.FirstName = guestToPatch.FirstName;
            existingGuest.LastName = guestToPatch.LastName;
            existingGuest.Email = guestToPatch.Email;
            existingGuest.Phone = guestToPatch.Phone;
            existingGuest.DOB = guestToPatch.DOB;
            existingGuest.IsDeleted = guestToPatch.IsDeleted;
            
            await context.SaveChangesAsync();

            return existingGuest;
        }



        public async Task<Guest> DeleteAsync(int guestId)
        {
            var existingGuest = await context.Guests
                .FirstOrDefaultAsync(g => g.Id == guestId && !g.IsDeleted);

            if (existingGuest == null)
                return null;

            existingGuest.IsDeleted = true;

            context.Guests.Update(existingGuest);
            await context.SaveChangesAsync();

            return existingGuest;
        }



        public async Task<(List<Guest>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Guests.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var guests = await context.Guests.Skip(pageSize * (pageNumber - 1))
                                        .Take(pageSize)
                                        .ToListAsync();

            return (guests, paginationData);
        }



        public async Task<Guest> GetByIdAsync(int guestId)
        {
            var existingGuest = await context.Guests
                .FirstOrDefaultAsync(g => g.Id == guestId && !g.IsDeleted);

            if (existingGuest == null)
                return null;

            return existingGuest;
        }
    }
}
