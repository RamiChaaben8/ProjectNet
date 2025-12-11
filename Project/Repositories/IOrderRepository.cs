using DataBase;

namespace Project.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByID (int id);

        Task<List<Order>> GetByClientID (int id);

        Task<List<Order>> GetAllOrders();

        Task<Order> CreateOrder(Order order);

        Task<bool> UpdateOrder(Order order);

        Task<bool> UpdateOrderStatus(int id, string status);

        Task<bool> DeleteOrder(int id);
    }
}
