using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Core.Models
{
    public class Product : BaseEntity
    {

        [Required]
        [StringLength(50, ErrorMessage = "Name character maximum length is 50!")]
        public string Name { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public string CategoryID { get; set; }

        [ForeignKey("Shop")]
        [Display(Name = "Shop")]
        public string ShopID { get; set; } 

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [StringLength(100, ErrorMessage = "Description character maximum length is 100!")]
        public string Description { get; set; }

        //[Required]
        public string Image1 { get; set; }

        public string Image2 { get; set; }
        [NotMapped]
        public string _Image1 { get; set; }

        [NotMapped]
        public string _Image2 { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

        [Required]
        public string UserID { get; set; }


        //REFERENCES 
        public virtual ProductCategory Category { get; set; }
        public virtual Shop Shop { get; set; } 
        public virtual ICollection<Order> Orders { get; set; }

    }
}
