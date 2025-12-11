using DataBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Repositories;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository repository;

        public CartController(ICartRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            return Ok(await repository.GetAllCarts());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart(Cart cart)
        {
            var newCart = await repository.CreateCart(cart);
            return CreatedAtAction(nameof(GetCart), new { id = newCart.CartID }, newCart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var result = await repository.DeleteCart(id);
            if (result) return NoContent();
            return NotFound("n'existe pas");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            var cart = await repository.GetCart(id);
            if (cart == null) return NotFound("inexistant");
            return Ok(cart);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart(Cart cart)
        {
            var result = await repository.UpdateCart(cart);
            if (result) return Ok(result);
            return NotFound("erreur");
        }
    }
}
