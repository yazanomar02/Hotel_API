using AutoMapper;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRepository<RoomType, RoomTypeForUpdate> repository;
        private readonly IMapper mapper;

        public RoomTypesController(IRepository<RoomType, RoomTypeForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<RoomType>> CreatRoomTypeAsync(RoomTypeForCreate roomType)
        {
            if (roomType == null)
                return BadRequest();

            var newRoomType = new RoomType()
            {
               TypeName = roomType.TypeName,
               NumOfBeds = roomType.NumOfBeds
            };

            var createResult = await repository.CreateAsync(newRoomType);

            if (createResult != null)
                return CreatedAtRoute("GetRoomType", new { id = newRoomType.Id }, newRoomType);

            else
                return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{roomTypeId}")]
        public async Task<ActionResult<RoomType>> UpdateRoomTypeAsync(int roomTypeId, RoomTypeForUpdate roomType)
        {
            if (roomType != null)
            {
                var newRoomType = new RoomType()
                {
                    Id = roomTypeId,
                    TypeName = roomType.TypeName,
                    NumOfBeds = roomType.NumOfBeds,
                    IsDeleted = roomType.IsDeleted
                };

                var updateResult = await repository.UpdateAsync(newRoomType);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{roomTypeId}")]
        public async Task<ActionResult<RoomType>> PartiallyUpdateRoomType(
            int roomTypeId, [FromBody] JsonPatchDocument<RoomTypeForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedRoomType = await repository.PartiallyUpdateAsync(roomTypeId, patchDocument);

            if (updatedRoomType == null)
                return NotFound();

            return NoContent();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{roomTypeId}")]
        public async Task<ActionResult<RoomType>> DeleteRoomType(int roomTypeId)
        {

            var deletedRoomType = await repository.DeleteAsync(roomTypeId);

            if (deletedRoomType == null)
                return NotFound();

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<RoomTypeWithOut_Rooms>>> GetRoomTypes(
            int pageNumber, int pageSize
            )
        {
            var (roomType, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize);

            if (roomType == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var roomTypeWithOutRooms = mapper.Map<List<RoomTypeWithOut_Rooms>>(roomType);

            return Ok(roomTypeWithOutRooms);
        }



        [HttpGet("{roomTypeId}", Name = "GetRoomType")]
        public async Task<ActionResult<RoomType>> GetRoomType(int roomTypeId)
        {
            var roomType = await repository.GetByIdAsync(roomTypeId);

            if (roomType == null)
                return NotFound();

            return Ok(roomType);
        }
    }
}
