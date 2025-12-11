using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext context;

        public UserRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            await context.users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await context.users.FindAsync(id);
            if (user == null) { return false; }
            context.users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await context.users.ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            return await context.users.FindAsync(id);
        }

        public async Task<bool> UpdateUser(User user)
        {
            var u = await context.users.FindAsync(user.UserId);
            if (u == null) { return false; };
            u.Name = user.Name;
            u.Email = user.Email;
            u.Password = user.Password;
            u.Phone = user.Phone;
            u.Role = user.Role;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
