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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository repository;

        public PaymentController(IPaymentRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            return Ok(await repository.GetAllPayments());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payment payment)
        {
            var newPayment = await repository.CreatePayment(payment);
            return CreatedAtAction(nameof(GetPayment), new { id = newPayment.PaymentID }, newPayment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await repository.DeletePayment(id);
            if (result) return NoContent();
            return NotFound("n'existe pas");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await repository.GetPayment(id);
            if (payment == null) return NotFound("inexistant");
            return Ok(payment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayment(Payment payment)
        {
            var result = await repository.UpdatePayment(payment);
            if (result) return Ok(result);
            return NotFound("erreur");
        }
    }
}
