using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Models
{
    public class RequisitionSupplement
    {
        [Required]
        public string RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public RequisitionHeader RequisitionHeader { get; set; }

        public string FileLink { get; set; }
        public int Number { get; set; } 
    }
}
