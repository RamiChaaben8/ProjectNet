using DataBase;

namespace Project.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();

        Task<User> GetUser(int id);

        Task<User> CreateUser(User user);

        Task<bool> UpdateUser(User user);

        Task<bool> DeleteUser(int id);
    }
}
