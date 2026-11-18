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
                // Create a new list to store article references
                var articleIds = order.Receipt.Select(a => a.ArticleID).ToList();
                
                // Clear the receipt to avoid tracking issues with navigation properties
                order.Receipt.Clear();
                
                // Fetch the articles from database without their navigation properties being tracked
                foreach (var articleId in articleIds)
                {
                    // Get existing tracked entity or fetch from database
                    var trackedArticle = context.ChangeTracker.Entries<Article>()
                        .FirstOrDefault(e => e.Entity.ArticleID == articleId)?.Entity;
                    
                    if (trackedArticle == null)
                    {
                        // Fetch the article without loading navigation properties
                        var article = await context.Articles.FindAsync(articleId);
                        if (article != null)
                        {
                            // Detach the category to prevent tracking conflicts
                            var categoryEntry = context.Entry(article).Reference(a => a.Category);
                            if (categoryEntry.IsLoaded && article.Category != null)
                            {
                                context.Entry(article.Category).State = EntityState.Detached;
                            }
                            order.Receipt.Add(article);
                        }
                    }
                    else
                    {
                        order.Receipt.Add(trackedArticle);
                    }
                }
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
