using System.ComponentModel.DataAnnotations;
using WorkFlow.Models;

public class RequisitionApprovalActionViewModel
{
    [Required]
    public int ApprovalId { get; set; }

    [Required]
    public RequisitionAction NewAction { get; set; }

    public string Comment { get; set; }

    public string? SentTo { get; set; } // Include if needed for creation of new approvals

    public List<String>? UsersEmail { get; set; }
}
