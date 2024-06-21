using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application
{
    public class RepositoryHotel : IRepository<Hotel, HotelForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryHotel(HotelContext context)
        {
            this.context = context;
        }


        public async Task<Hotel> CreateAsync(Hotel hotel)
        {

            if (hotel != null)
            {
                context.Hotels.Add(hotel);
                await context.SaveChangesAsync();

                return hotel;
            }
            return null;
        }
        

        public async Task<Hotel> UpdateAsync(Hotel hotel)
        {
            if (hotel != null)
            {
                // التحقق من وجود الفندق في قاعدة البيانات
                var existingHotel = await context.Hotels
                                          .FirstOrDefaultAsync(h => h.Id == hotel.Id && !h.IsDeleted);

                if (existingHotel != null)
                {
                    existingHotel.Name = hotel.Name;
                    existingHotel.Email = hotel.Email;
                    existingHotel.Address = hotel.Address;
                    existingHotel.Phone = hotel.Phone;
                    existingHotel.IsDeleted = hotel.IsDeleted;

                    context.Hotels.Update(existingHotel);
                    await context.SaveChangesAsync();

                    return existingHotel;
                }
            }

            await Console.Out.WriteLineAsync("Hotel not found!!");
            return null;
        }


        public async Task<Hotel> PartiallyUpdateAsync(int hotelId, JsonPatchDocument<HotelForUpdate> patchDocument)
        {
            var existingHotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);

            if (existingHotel == null)
                return null;

            var hotelToPatch = new HotelForUpdate
            {
                Name = existingHotel.Name,
                Email = existingHotel.Email,
                Phone = existingHotel.Phone,
                Address = existingHotel.Address,
                IsDeleted = existingHotel.IsDeleted,
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(hotelToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }

            existingHotel.Name = hotelToPatch.Name;
            existingHotel.Email = hotelToPatch.Email;
            existingHotel.Phone = hotelToPatch.Phone;
            existingHotel.Address = hotelToPatch.Address;
            existingHotel.IsDeleted = hotelToPatch.IsDeleted;

            await context.SaveChangesAsync();

            return existingHotel;
        }



        public async Task<Hotel> DeleteAsync(int hotelId)
        {
            var existingHotel = await context.Hotels
                .FirstOrDefaultAsync(e => e.Id == hotelId && !e.IsDeleted);

            if (existingHotel == null)
                return null;

            existingHotel.IsDeleted = true;

            context.Hotels.Update(existingHotel);
            await context.SaveChangesAsync();

            return existingHotel;
        }


        public async Task<(List<Hotel>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Rooms.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // بناء ال query
            var query = context.Hotels as IQueryable<Hotel>;

            if (!keyword.IsNullOrEmpty())
                query = query.Where(p => p.Address.ToLower().Contains(keyword.ToLower()));

            var hotels = await query.OrderBy(p => p.Name)
                                       .Skip(pageSize * (pageNumber - 1))
                                       .Take(pageSize)
                                       .ToListAsync();

            return (hotels, paginationData);
        }


        public async Task<Hotel> GetByIdAsync(int hotelId)
        {
            var hotel = await context.Hotels
                        .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted);

            if (hotel == null)
                return null;

            return hotel;
        }
    }
}
