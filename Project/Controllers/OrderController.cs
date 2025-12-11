using DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Project.Repositories;
using Project.DTO;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository repository;
        private readonly IClientRepository clientRepository;

        public OrderController(IOrderRepository repository, IClientRepository clientRepository) 
        {
            this.repository = repository;
            this.clientRepository = clientRepository;
        }
        
        [HttpGet]
        [Authorize(Roles = "Item-Admin,Admin,admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await repository.GetAllOrders();
            var ordersDto = orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.OrderID,
                TotalPrice = o.TotalPrice,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Receipt = o.Receipt,
                ClientID = o.ClientID,
                ClientEmail = o.Client?.Email ?? "N/A",
                ClientName = o.Client?.Name ?? "N/A"
            }).ToList();
            
            return Ok(ordersDto);
        }
        
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                email = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? 
                        User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized("User email not found in claims");
            }

            var client = await clientRepository.GetClientByEmail(email);
            
            if (client == null)
            {
                return NotFound("Client not found");
            }

            var orders = await repository.GetByClientID(client.UserId);
            var ordersDto = orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.OrderID,
                TotalPrice = o.TotalPrice,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Receipt = o.Receipt,
                ClientID = o.ClientID,
                ClientEmail = client.Email,
                ClientName = client.Name
            }).ToList();
            
            return Ok(ordersDto);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Project.DTO.CreateOrderDTO orderDto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                // Fallback for when ClaimTypes.Name isn't populated as expected (e.g. some JWT configs)
                email = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? 
                        User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized("User email not found in claims");
            }

            var client = await clientRepository.GetClientByEmail(email);
            
            if (client == null)
            {
                 // Create a new client record for this user if one doesn't exist
                 client = new DataBase.Client
                 {
                     Email = email,
                     Name = email,
                     Password = "Hidden",
                     Phone = "",
                     Adresse = "",
                     Role = "Client"
                 };
                 await clientRepository.CreateNewClient(client);
            }

            var order = new Order
            {
                TotalPrice = orderDto.TotalPrice,
                OrderDate = orderDto.OrderDate,
                Status = "Pending", // Set default status to Pending
                Receipt = orderDto.Receipt,
                ClientID = client.UserId
            };

            return Ok(await repository.CreateOrder(order));
        }
        
        [HttpDelete]
        [Authorize(Roles = "Item-Admin,Admin,admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await repository.DeleteOrder(id);
            if (result) return Ok(result);
            return NotFound("n'existe pas");
        }
        
        [HttpGet("client/{ClientID}")]
        public async Task<IActionResult> GetByClientID(int ClientID)
        {
            var result = await repository.GetByClientID(ClientID);
            if(result.Any()) return Ok(result);
            return NotFound("empty");
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByID(int id)
        {
            var order = await repository.GetOrderByID(id);
            if (order == null) return NotFound("n'existe pas");
            return Ok(order);
        }
        
        [HttpPut("status")]
        [Authorize(Roles = "Item-Admin,Admin,admin")] // Allow lowercase admin
        public async Task<IActionResult> UpdateOrderStatus([FromBody] Project.DTO.UpdateOrderStatusDTO dto)
        {
            try
            {
                var result = await repository.UpdateOrderStatus(dto.OrderID, dto.Status);
                if (result) return Ok(result);
                return NotFound("Order not found");
            }
            catch (Exception ex)
            {
                 return BadRequest($"Error: {ex.Message} {ex.InnerException?.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Item-Admin,Admin,admin")]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            var result = await repository.UpdateOrder(order);
            if (result) Ok(result);
            return NotFound("erreur");
        }
    }
}
