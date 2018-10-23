using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrillOut.Models
{
    public class Events
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Employee")]
        [Display(Name = "Employee Id")]
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Display(Name = "City, State, Zip")]
        public string CityStateZip { get; set; }

        [Display(Name = "Has been delivered")]
        public bool IsDelivered { get; set; }

        [Display(Name = "Has been picked up")]
        public bool IsPickedUp { get; set; }
    }
}
