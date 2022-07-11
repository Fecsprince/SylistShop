using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class Shop : BaseEntity
    {

        [Required]
        [StringLength(50, ErrorMessage = "Name character maximum length is 50!")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Address character maximum length is 100!")]
        public string Location { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string LGA { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostCode { get; set; } 

        [Required]
        [MaxLength(11, ErrorMessage = "Contact1 character length should be 11!")]
        [MinLength(11, ErrorMessage = "Contact1 character length should be 11!")]
        [Display(Name = "Primary Contact")]
        public string Contact1 { get; set; }

        [Required]
        [MaxLength(11, ErrorMessage = "Contact2 character length should be 11!")]
        [MinLength(11, ErrorMessage = "Contact2 character length should be 11!")]
        [Display(Name = "Secondary Contact")]
        public string Contact2 { get; set; }

        public string Image1 { get; set; }

        public string Image2 { get; set; }
        [NotMapped]
        public string _Image1 { get; set; }
        [NotMapped]
        public string _Image2 { get; set; }
        [Required]
        [Display(Name = "User")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "Booking Days")]
        public string Days { get; set; }

        [Required]
        [Display(Name = "Opening Hour")]
        public string OpeningHour { get; set; }

        [Required]
        [Display(Name = "Closing Hour")]
        public string ClosingHour { get; set; }



        //REFERENCES
        public virtual ICollection<Service> Services { get; set; }

    }
}
