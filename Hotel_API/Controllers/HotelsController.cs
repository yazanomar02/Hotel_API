using AutoMapper;
using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IRepository<Hotel, HotelForUpdate> repository;
        private readonly IMapper mapper;

        public HotelsController(IRepository<Hotel, HotelForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Hotel>> CreateHotelAsync(HotelForCreate hotel)
        {
            if (hotel == null)
                return BadRequest();

            var newHotel = new Hotel()
            {
                Name = hotel.Name,
                Email = hotel.Email,
                Phone = hotel.Phone,
                Address = hotel.Address
            };

            var createResult = await repository.CreateAsync(newHotel);

            if (createResult == null)
                return BadRequest();

            return CreatedAtRoute("GetHotel", new { id = newHotel.Id }, newHotel);
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{hotelId}")]
        public async Task<ActionResult<Hotel>> UpdateHotelAsync(int hotelId, HotelForUpdate hotel)
        {
            if (hotel != null)
            {
                var newHotel = new Hotel()
                {
                    Id = hotelId,
                    Name = hotel.Name,
                    Email = hotel.Email,
                    Phone = hotel.Phone,
                    Address = hotel.Address,
                    IsDeleted = hotel.IsDeleted
                };

                var updateResult = await repository.UpdateAsync(newHotel);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{hotelId}")]
        public async Task<ActionResult<Hotel>> PartiallyUpdateHotel(
            int hotelId, [FromBody] JsonPatchDocument<HotelForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedHotel = await repository.PartiallyUpdateAsync(hotelId, patchDocument);

            if (updatedHotel == null)
                return NotFound();

            return NoContent();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{hotelId}")] 
        public async Task<ActionResult<Hotel>> DeleteHotel(int hotelId)
        {

            var deletedHotel = await repository.DeleteAsync(hotelId);

            if (deletedHotel == null)
                return NotFound();

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<HotelWithOut_Rooms_Employees>>> GetHotels(
            int pageNumber, int pageSize, string? keyword
            )
        {
            var (hotels, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize, keyword);

            if (hotels == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var hotelWithOutRoomsEmployees = mapper.Map<List<HotelWithOut_Rooms_Employees>>(hotels);

            return Ok(hotelWithOutRoomsEmployees);
        }



        [HttpGet("{hotelId}", Name = "GetHotel")]
        public async Task<ActionResult<Hotel>> GetHotel(int hotelId)
        {
            var hotel = await repository.GetByIdAsync(hotelId);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }
    }
}
