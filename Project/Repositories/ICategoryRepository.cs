using DataBase;

namespace Project.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories();

        Task<Category> GetCategoryByName(string name);

        Task<Category> GetCategoryByID(int id);

        Task<Category> CreateCategory(Category category);

        Task<bool> UpdateCategory(Category category);

        Task<bool> DeleteCategory(int id);
    }
}
