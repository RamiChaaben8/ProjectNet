using DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Repositories;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "admin")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository repository;

        public ArticleController(IArticleRepository repository) {
            this.repository = repository;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllArticals()
        {
            return Ok(await repository.GetAllArticals());
        }

        [HttpPost]

        public async Task<IActionResult> CreateArticle (Article article)
        {
            var articles = await repository.GetAllArticals();
            if (articles.Any(a => a.Name.Equals(article.Name))) { return BadRequest("existe"); }
            var newart = await repository.CreateArticle(article);
            return CreatedAtAction(nameof(GetArticle), new { id = newart.ArticleID }, newart);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await repository.DeleteArticle(id);
            if (result) return NoContent();
            return NotFound("n'existe pas");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = await repository.GetArticle(id);
            if (article == null) return NotFound("inexistant");
            return Ok(article);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateArticle(Article article)
        {
            var result = await repository.UpdateArticle(article);
            if (result) return Ok(result);
            return NotFound("erreur");
        }
    }
}
