using DataBase;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationContext context;
        public ClientRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public async Task<Client> CreateNewClient(Client client)
        {
            await context.Clients.AddAsync(client);
            await context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteNewClient(int id)
        {
            var cli = await context.Clients.FindAsync(id);
            if (cli == null) { return false; }
            context.Clients.Remove(cli);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Client>> GetAllClients()
        {
            return await context.Clients.ToListAsync();
        }

        public async Task<Client> GetClient(int id)
        {
            return await context.Clients.FindAsync(id);
        }

        public async Task<bool> UpdateNewClient(Client client)
        {
            var cli = await context.Clients.FindAsync (client.UserId);
            if (cli == null) { return false; }
            cli.Name = client.Name;
            cli.Email = client.Email;
            cli.Password = client.Password;
            cli.Phone = client.Phone;
            cli.Adresse = client.Adresse;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Client?> GetClientByEmail(string email)
        {
            return await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
