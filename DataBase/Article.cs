using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Article
    {
        [Key]
        public int ArticleID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public float Price { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey ("CategoryID")]

        public virtual Category? Category { get; set; }

        public string? ImageUrl { get; set; }
    }
}
