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
    public class PaymentsController : ControllerBase
    {
        private readonly IRepository<Payment, PaymentForUpdate> repository;
        private readonly IMapper mapper;

        public PaymentsController(IRepository<Payment, PaymentForUpdate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePaymentAsync(PaymentForCreate payment)
        {
            if (payment == null)
                return BadRequest();

            var newPayment = new Payment()
            {
                TotalAmount = payment.TotalAmount,
                BookingId = payment.BookingId,
                CreatedDate = DateTime.UtcNow
            };

            var createResult = await repository.CreateAsync(newPayment);

            if (createResult == null)
                return BadRequest();

            return CreatedAtRoute("GetPayment", new { id = newPayment.Id }, newPayment);
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPut("{paymentId}")]
        public async Task<ActionResult<Payment>> UpdatePaymentAsync(int paymentId, PaymentForUpdate payment)
        {
            if (payment != null)
            {
                var newPayment = new Payment()
                {
                    Id = paymentId,
                    TotalAmount = payment.TotalAmount,
                    CreatedDate = payment.CreatedDate,
                    IsDeleted = payment.IsDeleted,
                    BookingId = payment.BookingId
                };

                var updateResult = await repository.UpdateAsync(newPayment);

                if (updateResult != null)
                    return NoContent();

                else
                    return NotFound();
            }

            return BadRequest();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpPatch("{paymentId}")]
        public async Task<ActionResult<Payment>> PartiallyUpdatePayment(
            int paymentId, [FromBody] JsonPatchDocument<PaymentForUpdate> patchDocument
            )
        {
            if (patchDocument == null)
                return BadRequest("Patch document is null.");

            var updatedPayment = await repository.PartiallyUpdateAsync(paymentId, patchDocument);

            if (updatedPayment == null)
                return NotFound();

            return NoContent();
        }



        [Authorize(Policy = "userShouldeBeUserAndFromCoures11")]
        [HttpDelete("{paymentId}")]
        public async Task<ActionResult<Payment>> DeletePayment(int paymentId)
        {

            var deletedPayment = await repository.DeleteAsync(paymentId);

            if (deletedPayment == null)
                return NotFound();

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<List<PaymentWithOut_Booking>>> GetPayments(
            int pageNumber, int pageSize
            )
        {
            var (payments, paginationMetaData) = await repository.GetAllAsync(pageNumber, pageSize);

            if (payments == null)
                return NotFound();


            // إضافة رأس مخصص (Custom Header)
            // يحمل معلومات عن بيانات التصفح (pagination metadata).
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));


            var paymentWithOutBooking = mapper.Map<List<PaymentWithOut_Booking>>(payments);

            return Ok(paymentWithOutBooking);
        }



        [HttpGet("{paymentId}", Name = "GetPayment")]
        public async Task<ActionResult<Payment>> GetPayment(int paymentId)
        {
            var payment = await repository.GetByIdAsync(paymentId);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }
    }
}
