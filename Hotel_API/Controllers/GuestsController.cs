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
    public class GuestsController : ControllerBase
    {
        private readonly IRepository<Guest, GuestForUpdate> repository;
        private readonly IMapper mapper;

        public GuestsController(IRepository<Guest, GuestForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Guest>> CreatGuestAsync(GuestForCreate guest)
        {
            if (guest == null)
                return BadRequest();

            var newGuest = new Guest()
            {
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone,
                DOB = guest.DOB,
                BookingId = guest.BookingId
            };

            var createResult = await repository.CreateAsync(newGuest);

            if (createResult != null)
                return CreatedAtRoute("GetGuest", new { id = newGuest.Id }, newGuest);

            else
                return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{guestId}")]
        public async Task<ActionResult<Guest>> UpdateGuestAsync(int guestId, GuestForUpdate guest)
        {
            if (guest != null)
            {
                var newGuest = new Guest()
                {
                    Id = guestId,
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    Email = guest.Email,
                    Phone = guest.Phone,
                    DOB = guest.DOB,
                    IsDeleted = guest.IsDeleted,
                    BookingId = guest.BookingId
                };

                var updateResult = await repository.UpdateAsync(newGuest);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{guestId}")]
        public async Task<ActionResult<Guest>> PartiallyUpdateGuest(
            int guestId, JsonPatchDocument<GuestForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedGuest = await repository.PartiallyUpdateAsync(guestId, patchDocument);

            if (updatedGuest == null)
                return NotFound();

            return NoContent();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{guestId}")]
        public async Task<ActionResult<Guest>> DeleteGuest(int guestId)
        {
            var deletedGuest = await repository.DeleteAsync(guestId);

            if (deletedGuest == null)
                return NotFound();

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<GuestWithOut_Booking>>> GetGuests(
            int pageNumber, int pageSize
            )
        {
            var (guests, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize);

            if (guests == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var guestWithOutBooking = mapper.Map<List<GuestWithOut_Booking>>(guests);

            return Ok(guestWithOutBooking);
        }



        [HttpGet("{guestId}", Name = "GetGuest")]
        public async Task<ActionResult<Guest>> GetGuest(int guestId)
        {
            var guest = await repository.GetByIdAsync(guestId);

            if (guest == null)
                return NotFound();

            return Ok(guest);
        }
    }
}
