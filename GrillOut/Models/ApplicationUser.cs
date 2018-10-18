using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrillOut.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Display(Name = "Customer")]
        public string Name { get; set; }

        [NotMapped]
        public bool isEmployee { get; set; }
    }
}
