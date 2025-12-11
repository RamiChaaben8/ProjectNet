using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationContext context;

        public CartRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Cart> CreateCart(Cart cart)
        {
            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteCart(int id)
        {
            var cart = await context.Carts.FindAsync(id);
            if (cart == null) { return false; }
            context.Carts.Remove(cart);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Cart>> GetAllCarts()
        {
            return await context.Carts.ToListAsync();
        }

        public async Task<Cart> GetCart(int id)
        {
            return await context.Carts.FindAsync(id);
        }

        public async Task<bool> UpdateCart(Cart cart)
        {
            var c = await context.Carts.FindAsync(cart.CartID);
            if (c == null) { return false; };
            c.TotalPrice = cart.TotalPrice;
            c.UserID = cart.UserID;
            // Note: Updating related entities like listOfArticles might require more complex logic depending on requirements
            // For now, we follow the pattern of simple property updates
            await context.SaveChangesAsync();
            return true;
        }
    }
}
