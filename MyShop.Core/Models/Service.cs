using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models

{
    public class Service : BaseEntity
    {
        //[Key]
        //public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Name character maximum length is 50!")]
        public string Name { get; set; }

        [ForeignKey("Shop")]
        public string ShopID { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public string Image1 { get; set; }

        public string Image2 { get; set; }




        //REFERENCES
        public virtual Shop Shop { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
