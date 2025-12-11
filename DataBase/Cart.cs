using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        public float TotalPrice { get; set; }

        public int UserID { get; set; }

        [ForeignKey ("UserID")]

        public virtual Client Client { get; set; }

        public List <Article> listOfArticles { get; set; } = new List<Article> ();
    }
}
