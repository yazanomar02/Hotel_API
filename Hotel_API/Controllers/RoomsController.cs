using AutoMapper;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRepository<Room, RoomForUpdate> repository;
        private readonly IMapper mapper;

        public RoomsController(IRepository<Room, RoomForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Room>> CreatRoomAsync(RoomForCreate room)
        {
            if (room == null)
                return BadRequest();

            var newRoom = new Room()
            {
                Number = room.Number,
                FloorNumber = room.FloorNumber,
                Status = room.Status,
                HotelId = room.HotelId,
                RoomTypeId = room.RoomTypeId,
            };

            var createResult = await repository.CreateAsync(newRoom);

            if (createResult != null)
                return CreatedAtRoute("GetRoom", new { id = newRoom.Id }, newRoom);

            else
                return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{roomId}")]
        public async Task<ActionResult<Room>> UpdateRoomAsync(int roomId, RoomForUpdate room)
        {
            if (room != null)
            {
                var newRoom = new Room()
                {
                    Id = roomId,
                    Number = room.Number,
                    FloorNumber = room.FloorNumber,
                    Status = room.Status,
                    HotelId = room.HotelId,
                    RoomTypeId = room.RoomTypeId,
                    IsDeleted = room.IsDeleted
                };

                var updateResult = await repository.UpdateAsync(newRoom);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{roomId}")]
        public async Task<ActionResult<Room>> PartiallyUpdateRoom(int roomId, JsonPatchDocument<RoomForUpdate> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("Patch document is null.");
            }

            var updatedRoom = await repository.PartiallyUpdateAsync(roomId, patchDocument);

            if (updatedRoom == null)
            {
                return NotFound();
            }

            return NoContent();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{roomId}")]
        public async Task<ActionResult<Room>> DeleteRoom(int roomId)
        {
            var deletedRoom = await repository.DeleteAsync(roomId);

            if (deletedRoom == null)
            {
                return NotFound();
            }

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<RoomWithOut_Hotel_RoomType_Booking>>> GetRooms(int pageNumber, int pageSize)
        {
            var (rooms, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize);

            if (rooms == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var roomWithOutHotelRoomTypeBooking = mapper.Map<List<RoomWithOut_Hotel_RoomType_Booking>>(rooms);

            return Ok(roomWithOutHotelRoomTypeBooking);
        }



        [HttpGet("{roomId}", Name = "GetRoom")]
        public async Task<ActionResult<Room>> GetRoom(int roomId)
        {
            var room = await repository.GetByIdAsync(roomId);

            if (room == null)
                return NotFound();

            return Ok(room);
        }
    }
}
