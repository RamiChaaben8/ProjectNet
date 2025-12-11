using DataBase;

namespace Project.Repositories
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllCarts();

        Task<Cart> GetCart(int id);

        Task<Cart> CreateCart(Cart cart);

        Task<bool> UpdateCart(Cart cart);

        Task<bool> DeleteCart(int id);
    }
}
