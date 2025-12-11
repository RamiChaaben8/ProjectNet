using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationContext context;

        public PaymentRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Payment> CreatePayment(Payment payment)
        {
            await context.Payment.AddAsync(payment);
            await context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeletePayment(int id)
        {
            var payment = await context.Payment.FindAsync(id);
            if (payment == null) { return false; }
            context.Payment.Remove(payment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await context.Payment.ToListAsync();
        }

        public async Task<Payment> GetPayment(int id)
        {
            return await context.Payment.FindAsync(id);
        }

        public async Task<bool> UpdatePayment(Payment payment)
        {
            var p = await context.Payment.FindAsync(payment.PaymentID);
            if (p == null) { return false; };
            p.PaymentType = payment.PaymentType;
            p.PaymentStatus = payment.PaymentStatus;
            p.OrderID = payment.OrderID;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
