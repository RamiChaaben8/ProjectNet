using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationContext context;
        public CategoryRepository(ApplicationContext context) {
            this.context = context;
        }
        public async Task<Category> CreateCategory(Category category)
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) { return false; }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByID(int id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            return await context.Categories.FindAsync(name);
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            var  cat = await context.Categories.FindAsync(category.CategoryID);
            if (cat == null) { return false;};
            cat.Name = category.Name;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
