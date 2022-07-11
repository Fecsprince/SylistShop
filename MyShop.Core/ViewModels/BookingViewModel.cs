using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.ViewModels
{
    public class BookingViewModel
    {
        public string Id { get; set; }

        public string Service { get; set; }
        public string CustomerPhone { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; } 
        public string Shop { get; set; }
    }
}
