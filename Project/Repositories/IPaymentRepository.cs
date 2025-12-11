using DataBase;

namespace Project.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPayments();

        Task<Payment> GetPayment(int id);

        Task<Payment> CreatePayment(Payment payment);

        Task<bool> UpdatePayment(Payment payment);

        Task<bool> DeletePayment(int id);
    }
}
