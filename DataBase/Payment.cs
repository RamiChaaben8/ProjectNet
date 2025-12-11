using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Payment
    {
        [Key]
        public int  PaymentID { get; set; }

        public string PaymentType { get; set; }

        public string PaymentStatus { get; set; }

        public int OrderID { get; set; }

        [ForeignKey ("OrderID")]

        public virtual Order Order { get; set; }
    }
}
