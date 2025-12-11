using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationContext context;

        public ArticleRepository(ApplicationContext context) {
            this.context = context;
        }

        public async Task<Article> CreateArticle(Article article)
        {
            await context.Articles.AddAsync(article);
            await context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteArticle(int id)
        {
            var art = await context.Articles.FindAsync(id);
            if (art == null) { return false; }
            context.Articles.Remove(art);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Article>> GetAllArticals()
        {
            return await context.Articles.Include(a => a.Category).ToListAsync();
        }

        public async Task<Article> GetArticle(int id)
        {
            return await context.Articles.Include(a => a.Category).FirstOrDefaultAsync(a => a.ArticleID == id);
        }

        public async Task<bool> UpdateArticle(Article article)
        {
            var art = await context.Articles.FindAsync(article.ArticleID);
            if (art == null) { return false; };
            art.Name = article.Name;
            art.Description = article.Description;
            art.Price = article.Price;
            art.CategoryID = article.CategoryID;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
