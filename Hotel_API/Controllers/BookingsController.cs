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
    public class BookingsController : ControllerBase
    {
        private readonly IRepository<Booking, BookingForUpdate> repository;
        private readonly IMapper mapper;

        public BookingsController(IRepository<Booking, BookingForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Booking>> CreatBookingAsync(BookingForCreate booking)
        {
            if (booking == null)
                return BadRequest();

            var newBooking = new Booking()
            {
                CheckinAt = DateTime.UtcNow,
                CheckoutAt = booking.CheckoutAt,
                Price = booking.Price,
                RoomId = booking.RoomId,
                EmployeeId = booking.EmployeeId
            };

            var createResult = await repository.CreateAsync(newBooking);

            if (createResult != null)
                return CreatedAtRoute("GetBooking", new { id = newBooking.Id }, newBooking);

            else
                return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{bookingId}")]
        public async Task<ActionResult<Booking>> UpdateBookingAsync(int bookingId, BookingForUpdate booking)
        {
            if (booking != null)
            {
                var newBooking = new Booking()
                {
                    Id = bookingId,
                    CheckinAt = booking.CheckinAt,
                    CheckoutAt = booking.CheckoutAt,
                    Price = booking.Price,
                    RoomId = booking.RoomId,
                    EmployeeId = booking.EmployeeId,
                    IsDeleted = booking.IsDeleted
                };

                var updateResult = await repository.UpdateAsync(newBooking);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{bookingId}")]
        public async Task<ActionResult<Booking>> PartiallyUpdateBooking(
            int bookingId, JsonPatchDocument<BookingForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedBooking = await repository.PartiallyUpdateAsync(bookingId, patchDocument);

            if (updatedBooking == null)
                return NotFound();

            return NoContent();
        }


        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{bookingId}")]
        public async Task<ActionResult<Booking>> DeleteBooking(int bookingId)
        {

            var deletedBooking = await repository.DeleteAsync(bookingId);

            if (deletedBooking == null)
                return NotFound();

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<BookingWithOut_Guest_Room_Employee_Payments>>> GetBookings(
            int pageNumber, int pageSize
            )
        {
            var (bookings, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize);

            if (bookings == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var bookingWithOutGuestRoomEmployeePayments = mapper.Map<List<BookingWithOut_Guest_Room_Employee_Payments>>(bookings);

            return Ok(bookingWithOutGuestRoomEmployeePayments);
        }



        [HttpGet("{bookingId}", Name = "GetBooking")]
        public async Task<ActionResult<Booking>> GetHotel(int bookingId)
        {
            var booking = await repository.GetByIdAsync(bookingId);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }
    }
}
