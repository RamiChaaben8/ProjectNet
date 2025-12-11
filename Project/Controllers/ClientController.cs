using DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Repositories;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository repository;
        public ClientController(IClientRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            return Ok(await repository.GetAllClients());
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewClient(Client client)
        {
            var clients = await repository.GetAllClients();
            if (clients.Any(c => c.Name.Equals(client.Name))) return BadRequest("existe");
            var newclient = await repository.CreateNewClient(client);
            return CreatedAtAction(nameof(GetAllClients), new {id=newclient.UserId}, newclient);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteNewClient(int id)
        {
            var result = await repository.DeleteNewClient(id);
            if(result) return Ok(result);
            return NotFound("n'existe pas");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await repository.GetClient(id);
            if (client == null) return NotFound("n'existe pas");
            return Ok(client);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateNewClient(Client client)
        {
            var result = await repository.UpdateNewClient(client);
            if (result) return Ok(result);
            return BadRequest("erreur");
        }
    }
}
