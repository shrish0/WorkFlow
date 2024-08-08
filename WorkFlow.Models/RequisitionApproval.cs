using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Models
{
    public enum RequisitionAction
    {
        Pending,
        Rejected,
        NeedUpdation
    }

    public class RequisitionApproval
    {
        [Required]
        public string RequisitionId { get; set; }

        [ForeignKey("RequisitionId")]
        public RequisitionHeader? RequisitionHeader { get; set; }

        public string? SentTo { get; set; }
        public string? SentBy { get; set; }


        [Required]
        public RequisitionAction Action { get; set; } = RequisitionAction.Pending; // Use enum

        public string? Comment { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;
    }
}
