using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrillOut.Models
{
    public class Customer
    {

        [Key]
        public int CustomerID { get; set; }

        [Display(Name = "Your Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Display(Name = "City, State, Zip")]
        public string CityStateZip { get; set; }

        [Display(Name = "Has been dropped off")]
        public bool IsDelivered { get; set; }

        [Display(Name = "Has been picked up")]
        public bool IsPickedUp { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
