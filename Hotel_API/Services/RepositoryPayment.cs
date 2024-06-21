using Data;
using Domain.Entites;
using Domain.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Hotel_API.Services
{
    public class RepositoryPayment : IRepository<Payment, PaymentForUpdate>
    {
        private readonly HotelContext context;

        public RepositoryPayment(HotelContext context)
        {
            this.context = context;
        }


        public async Task<Payment> CreateAsync(Payment payment)
        {

            if (payment != null)
            {

                var existingBooking = await context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == payment.BookingId && !b.IsDeleted);

                if (existingBooking != null)
                {
                    existingBooking.Price = existingBooking.Price + payment.TotalAmount;

                    context.Bookings.Update(existingBooking);
                    context.Payments.Add(payment);
                    await context.SaveChangesAsync();

                    return payment;
                }
            }
            return null;
        }


        public async Task<Payment> UpdateAsync(Payment payment)
        {
            if (payment != null)
            {
                // التحقق من وجود الدفع في قاعدة البيانات
                var existingِPayment = await context.Payments
                                          .FirstOrDefaultAsync(p => p.Id == payment.Id && !p.IsDeleted);

                if (existingِPayment != null)
                {
                    // التحقق من الحجز الجديد إن كان موجود أم لا
                    var existingBooking = await context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == payment.BookingId && !b.IsDeleted);

                    // إذا كان الحجز موجود نقوم بحساب المبلغ الاجمالي الجديد 
                    if (existingBooking != null)
                    {
                        var oldTotalAmount = existingِPayment.TotalAmount;
                        var newTotalAmount = payment.TotalAmount;

                        double difference;
                        double finalTotalAmount;

                        if (newTotalAmount > oldTotalAmount)
                        {
                            difference = newTotalAmount - oldTotalAmount;
                            finalTotalAmount = oldTotalAmount + difference;
                        }
                        else if (newTotalAmount < oldTotalAmount)
                        {
                            difference = oldTotalAmount - newTotalAmount;
                            finalTotalAmount = oldTotalAmount - difference;
                        }
                        else
                        {
                            finalTotalAmount = 0;
                        }

                        existingBooking.Price = existingBooking.Price - existingِPayment.TotalAmount;
                        existingBooking.Price = existingBooking.Price + finalTotalAmount;

                        context.Bookings.Update(existingBooking);
                    }

                    else
                    {
                        await Console.Out.WriteLineAsync("New Booking not found!!");
                        return null;
                    }

                    existingِPayment.TotalAmount = payment.TotalAmount;
                    existingِPayment.CreatedDate = payment.CreatedDate;
                    existingِPayment.IsDeleted = payment.IsDeleted;

                    context.Payments.Update(existingِPayment);
                    await context.SaveChangesAsync();

                    return existingِPayment;
                }
            }

            await Console.Out.WriteLineAsync("Payment not found!!");
            return null;
        }


        public async Task<Payment> PartiallyUpdateAsync(int paymentId, JsonPatchDocument<PaymentForUpdate> patchDocument)
        {
            var existingPayment = await context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);

            if (existingPayment == null)
                return null;

            var paymentToPatch = new PaymentForUpdate
            {
                TotalAmount = existingPayment.TotalAmount,
                CreatedDate = existingPayment.CreatedDate,
                IsDeleted = existingPayment.IsDeleted,
                BookingId = existingPayment.BookingId
            };

            try
            {
                // تطبيق التعديلات على الكائن
                patchDocument.ApplyTo(paymentToPatch);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying the patch document: {ex}");
                return null;
            }


            existingPayment.TotalAmount = paymentToPatch.TotalAmount;
            existingPayment.IsDeleted = paymentToPatch.IsDeleted;
            existingPayment.CreatedDate = paymentToPatch.CreatedDate;

            await context.SaveChangesAsync();

            return existingPayment;
        }



        public async Task<Payment> DeleteAsync(int paymentId)
        {
            var existingPayment = await context.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);

            if (existingPayment == null)
                return null;

            existingPayment.IsDeleted = true;

            context.Payments.Update(existingPayment);
            await context.SaveChangesAsync();

            return existingPayment;
        }


        public async Task<(List<Payment>, PaginationMetaData)> GetAllAsync(
            int pageNumber, int pageSize, string keyword = null
            )
        {
            // حساب عدد الغرف الكلّي
            var totalItemCount = await context.Payments.CountAsync();

            // نمط تقسيم صفحات العرض
            var paginationData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // ضبط عرض التقسيم للصفحات
            var payments = await context.Payments.Skip(pageSize * (pageNumber - 1))
                                        .Take(pageSize)
                                        .ToListAsync();

            return (payments, paginationData);
        }


        public async Task<Payment> GetByIdAsync(int paymentId)
        {
            var payment = await context.Payments
                        .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);

            if (payment == null)
                return null;

            return payment;
        }
    }
}
