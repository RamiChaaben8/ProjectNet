using DataBase;

namespace Project.DTO
{
    public class OrderResponseDTO
    {
        public int OrderID { get; set; }
        public float TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<Article> Receipt { get; set; } = new List<Article>();
        public int ClientID { get; set; }
        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
    }
}
