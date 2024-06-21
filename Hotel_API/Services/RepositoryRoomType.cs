using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Hotel_API.Services
{
    public class RepositoryRoomType : IRepository<RoomType, RoomTypeForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryRoomType(HotelContext context)
        {
            this.context = context;
        }



        public async Task<RoomType> CreateAsync(RoomType roomType)
        {

            if (roomType != null)
            {
                context.RoomTypes.Add(roomType);
                await context.SaveChangesAsync();

                return roomType;
            }
            return null;
        }



        public async Task<RoomType> UpdateAsync(RoomType roomType)
        {
            if (roomType != null)
            {
                // التحقق من وجود الفندق في قاعدة البيانات
                var existingRoomType = await context.RoomTypes
                                          .FirstOrDefaultAsync(rt => rt.Id == roomType.Id && !rt.IsDeleted);

                if (existingRoomType != null)
                {
                    existingRoomType.TypeName = roomType.TypeName;
                    existingRoomType.NumOfBeds = roomType.NumOfBeds;
                    existingRoomType.IsDeleted = roomType.IsDeleted;

                    context.RoomTypes.Update(existingRoomType);
                    await context.SaveChangesAsync();

                    return existingRoomType;
                }
            }

            await Console.Out.WriteLineAsync("RoomType not found!!");

            return null;
        }



        public async Task<RoomType> PartiallyUpdateAsync(int roomTypeId, JsonPatchDocument<RoomTypeForUpdate> patchDocument)
        {
            var existingRoomType = await context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomTypeId);

            if (existingRoomType == null)
                return null;

            var roomTpyeToPatch = new RoomTypeForUpdate
            {
                TypeName = existingRoomType.TypeName,
                NumOfBeds = existingRoomType.NumOfBeds,
                IsDeleted = existingRoomType.IsDeleted,
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(roomTpyeToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }


            existingRoomType.TypeName = roomTpyeToPatch.TypeName;
            existingRoomType.NumOfBeds = roomTpyeToPatch.NumOfBeds;
            existingRoomType.IsDeleted = roomTpyeToPatch.IsDeleted;

            await context.SaveChangesAsync();

            return existingRoomType;
        }



        public async Task<RoomType> DeleteAsync(int roomTypeId)
        {
            var existingRoomType = await context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && !rt.IsDeleted);

            if (existingRoomType == null)
                return null;

            existingRoomType.IsDeleted = true;

            context.RoomTypes.Update(existingRoomType);
            await context.SaveChangesAsync();

            return existingRoomType;
        }



        public async Task<(List<RoomType>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.RoomTypes.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var roomType = await context.RoomTypes.Skip(pageSize * (pageNumber - 1))
                                        .Take(pageSize)
                                        .ToListAsync();

            return (roomType, paginationData);
        }



        public async Task<RoomType> GetByIdAsync(int roomTypeId)
        {
            var roomType = await context.RoomTypes
                        .FirstOrDefaultAsync(rt => rt.Id == roomTypeId && !rt.IsDeleted);

            if (roomType == null)
                return null;

            return roomType;
        }
    }
}
