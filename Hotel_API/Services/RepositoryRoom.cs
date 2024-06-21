using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Management
{
    public class RepositoryRoom : IRepository<Room, RoomForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryRoom(HotelContext context)
        {
            this.context = context;
        }

        public async Task<Room> CreateAsync(Room room)
        {
            // التحقق من وجود نوع الغرفة في قاعدة البيانات
            var existingRoomType = await context.RoomTypes
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(rt => rt.Id == room.RoomTypeId && !rt.IsDeleted);

            if (existingRoomType == null)
            {
                await Console.Out.WriteLineAsync("Room type not found..!");
                return null;
            }

            // التحقق من وجود الفندق في قاعدة البيانات
            var existingHotel = await context.Hotels
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(h => h.Id == room.HotelId && !h.IsDeleted);

            if (existingHotel == null)
            {
                await Console.Out.WriteLineAsync("The hotel does not exist..!");
                return null;
            }

            // إضافة الغرفة إلى قاعدة البيانات
            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();
            await Console.Out.WriteLineAsync("Room added successfully.");

            return room;
        }



        public async Task<Room> UpdateAsync(Room room)
        {

            // التحقق من وجود الغرفة في قاعدة البيانات
            var existingRoom = await context.Rooms
                               .FirstOrDefaultAsync(r => r.Id == room.Id && !r.IsDeleted);

            if (existingRoom == null)
            {
                await Console.Out.WriteLineAsync("The room does not found !!");
                return null;
            }

            if (room.RoomType != null)
            {
                // التحقق من وجود هذا النوع في قاعدة البيانات
                var existingRoomType = await context.RoomTypes
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(rt => rt.Id == room.RoomTypeId && !rt.IsDeleted);

                if (existingRoomType == null)
                {
                    await Console.Out.WriteLineAsync("Room type not found..!");
                    return null;
                }
            }
            //تحديث المعلومات
            existingRoom.Number = room.Number;
            existingRoom.FloorNumber = room.FloorNumber;
            existingRoom.Status = room.Status;
            existingRoom.IsDeleted = room.IsDeleted;
            existingRoom.HotelId = room.HotelId;
            existingRoom.RoomTypeId = room.RoomTypeId;

            context.Rooms.Update(existingRoom);
            await context.SaveChangesAsync();

            return existingRoom;
        }



        public async Task<Room> PartiallyUpdateAsync(int roomId, JsonPatchDocument<RoomForUpdate> patchDocument)
        {
            var existingRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

            if (existingRoom == null)
                return null;

            var roomToPatch = new RoomForUpdate
            {
                Number = existingRoom.Number,
                FloorNumber = existingRoom.FloorNumber,
                Status = existingRoom.Status,
                HotelId = existingRoom.HotelId,
                RoomTypeId = existingRoom.RoomTypeId,
                IsDeleted = existingRoom.IsDeleted
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(roomToPatch);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteAsync($"An error occurred while applying the patch document: {ex}");
                return null;
            }

            var checkHotel = await context.Hotels
                .FirstOrDefaultAsync(h => h.Id == roomToPatch.HotelId && !h.IsDeleted);

            if (checkHotel == null)
            {
                await Console.Out.WriteLineAsync("The new hotel is not available");
                return null;
            }

            var checkRoomType = await context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == roomToPatch.RoomTypeId && !rt.IsDeleted);

            if (checkRoomType == null)
            {
                await Console.Out.WriteLineAsync("The new room type is not available");
                return null;
            }

            existingRoom.Number = roomToPatch.Number;
            existingRoom.FloorNumber = roomToPatch.FloorNumber;
            existingRoom.Status = roomToPatch.Status;
            existingRoom.HotelId = roomToPatch.HotelId;
            existingRoom.RoomTypeId = roomToPatch.RoomTypeId;
            existingRoom.IsDeleted = roomToPatch.IsDeleted;
            
            await context.SaveChangesAsync();

            return existingRoom;
        }



        public async Task<Room> DeleteAsync(int roomId)
        {

            var existingRoom = await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);

            if (existingRoom == null)
                return null;

            existingRoom.IsDeleted = true;

            context.Rooms.Update(existingRoom);
            await context.SaveChangesAsync();

            return existingRoom;
        }



        public async Task<(List<Room>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Rooms.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var rooms = await context.Rooms.Skip(pageSize * (pageNumber - 1))
                                        .Take(pageSize)
                                        .ToListAsync();

            return (rooms, paginationData);
        }



        public async Task<Room> GetByIdAsync(int roomId)
        {

            var room = await context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);

            if (room == null)
                return null;

            return room;
        }
    }
}