using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlow.Models
{
    public class RequisitionBody
    {
        [Required]
        public string RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public RequisitionHeader? RequisitionHeader { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Description { get; set; }

        [Required]
        public bool hasAttachment { get; set; }
    }
}
