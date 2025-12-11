using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class AddCategoryRequestDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
