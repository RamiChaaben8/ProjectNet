using DataBase;
using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class CreateOrderDTO
    {
        public float TotalPrice { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public string Status { get; set; }
        
        public List<Article> Receipt { get; set; } = new List<Article>();
    }
}
