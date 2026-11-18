using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationContext context;

        public OrderRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public async Task<Order> CreateOrder(Order order)
        {
            // Handle existing articles properly to prevent tracking conflicts
            if (order.Receipt != null && order.Receipt.Any())
            {
                // Create a new list to avoid modifying collection during enumeration
                var trackedArticles = new List<Article>();
                
                foreach (var article in order.Receipt.ToList())
                {
                    // Check if the entity is already being tracked
                    var trackedEntity = context.ChangeTracker.Entries<Article>()
                        .FirstOrDefault(e => e.Entity.ArticleID == article.ArticleID);
                    
                    if (trackedEntity != null)
                    {
                        // Entity is already tracked, use the tracked instance
                        trackedArticles.Add(trackedEntity.Entity);
                    }
                    else
                    {
                        // Entity is not tracked, so attach it
                        context.Articles.Attach(article);
                        trackedArticles.Add(article);
                    }
                }
                
                // Replace the receipt list with the properly tracked articles
                order.Receipt = trackedArticles;
            }
            
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var ord = await context.Orders.FindAsync(id);
            if (ord == null) {return false;}
            context.Orders.Remove(ord);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetAllOrders() 
        {
            return await context.Orders
                .Include(o => o.Client)
                .ToListAsync();
        }
        
        public async Task<List<Order>> GetByClientID(int id) // fel chak ba3d nchofo
        {
            return await context.Orders
                .Where(o => o.ClientID == id)
                .Include(o => o.Client)
                .Include(o => o.Receipt)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByID(int id)
        {
            return await context.Orders.FindAsync(id);
        }

        public async Task<bool> UpdateOrder(Order order) // fel chak ba3d nchofo
        {
            var ord = await context.Orders.FindAsync(order.OrderID);
            if (ord == null) {return false;}
            ord.Status = order.Status;
            ord.Receipt = order.Receipt;
            ord.TotalPrice = order.TotalPrice;
            ord.OrderDate = order.OrderDate;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderStatus(int id, string status)
        {
            var ord = await context.Orders.FindAsync(id);
            if (ord == null) { return false; }
            ord.Status = status;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
