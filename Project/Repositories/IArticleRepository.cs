using DataBase;

namespace Project.Repositories
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetAllArticals();

        Task <Article> GetArticle(int id);

        Task<Article> CreateArticle(Article article);

        Task<bool> UpdateArticle(Article article);

        Task<bool> DeleteArticle(int id);
    }
}
