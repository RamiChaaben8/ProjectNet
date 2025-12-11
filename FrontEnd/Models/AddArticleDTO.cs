using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class AddArticleDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryID { get; set; }

        public string? ImageUrl { get; set; }
    }
}
