using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.Core.ViewModels
{
    public class IndexVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Service> services { get; set; } 
    }
}