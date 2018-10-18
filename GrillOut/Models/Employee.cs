using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrillOut.Models
{
    public class Employee
    {

        [Key]
        public int EmployeeId { get; set; }

        [Display(Name = "Your Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Is Assigned")]
        public bool IsAssigned { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
