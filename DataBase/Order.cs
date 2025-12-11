using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Order
    {
        //hello
        [Key]
        public int OrderID { get; set; }

        public float TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public List<Article> Receipt { get; set; } = new List<Article>();
        public int ClientID { get; set; }

        [ForeignKey ("ClientID")]

        public virtual Client? Client { get; set; }
    }
}
