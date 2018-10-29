using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrillOut.Models
{
    public class Package
    {
        [Key]
        public int PackageId { get; set; }

        [Display(Name = "GrillOut Package")]
        public string GrillOutPackage { get; set; }

        [Display(Name = "Basic GrillOut - $200")]
        public bool choseBasicGrillOut { get; set; }

        [Display(Name = "Average GrillOut - $400")]
        public bool choseAverageGrillOut { get; set; }

        [Display(Name = "Elite GrillOut - $750")]
        public bool choseEliteGrillOut { get; set; }

        [ForeignKey("Customer")]
        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
