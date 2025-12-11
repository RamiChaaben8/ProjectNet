using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public string? Address { get; set; }
    }
}
