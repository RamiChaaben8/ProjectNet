using DataBase;

namespace Project.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClients();

        Task<Client> GetClient(int id);

        Task<Client> CreateNewClient(Client client);

        Task<bool> UpdateNewClient(Client client);

        Task<bool> DeleteNewClient(int id);

        Task<Client?> GetClientByEmail(string email);

    }
}
