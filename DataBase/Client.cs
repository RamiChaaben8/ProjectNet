using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Client : User
    {
        public string Adresse { get; set; }

        public List<Order> OrderHistory { get; set; } = new List<Order>();
    }
}
