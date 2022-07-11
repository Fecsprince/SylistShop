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
    public class Booking : BaseEntity
    {


        [ForeignKey("Service")]
        [Display(Name = "Service")]
        public string ServiceID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Display(Name = "Appointment Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        //public string  Date { get; set; } 

        [Display(Name = "Appointment Time")]
        [Required]
        public string AppointmentTime { get; set; }

        [Display(Name = "Address")]
        [StringLength(100, ErrorMessage = "Address character maximum length is 100!")]
        public string Address { get; set; }
        [NotMapped]
        public string ShopName { get; set; } 


        //REFERENCES
        public virtual Service Service { get; set; }
    }
}
