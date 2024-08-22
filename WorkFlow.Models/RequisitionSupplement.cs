using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlow.Models
{
    public class RequisitionSupplement
    {
        [Key]
        public int SupplementId { get; set; } // Primary key

        [Required]
        [MaxLength(13)]
        public string RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public RequisitionHeader RequisitionHeader { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(10)]
        public string? FileAddedBy { get; set; }
        public string FileLink { get; set; }
        public int Number { get; set; }
    }
}
