using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlow.Models
{
    public class RequisitionBody
    {
        [Required]
        [MaxLength(13)]
        public string RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public RequisitionHeader? RequisitionHeader { get; set; }

        [Required]
        [MaxLength(100)] // Example length, adjust as needed
        public string Subject { get; set; }

        [MaxLength(250)] // Example length, adjust as needed
        public string? Description { get; set; }

        [Required]
        public bool HasAttachment { get; set; } = false; // Default to false
    }
}
