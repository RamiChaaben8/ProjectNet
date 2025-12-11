using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class SignupDTO
    {
        public string Name { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; }
    }
}
