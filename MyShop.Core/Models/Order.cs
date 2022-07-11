using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("Product")]
        [Display(Name = "Product")]
        public string ProductID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string OrderStatus { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        //REFERENCES
        public virtual Product Product { get; set; }
    }
}
