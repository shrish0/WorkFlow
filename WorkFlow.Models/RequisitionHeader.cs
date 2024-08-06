using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlow.Models
{
    public class RequisitionHeader
    {
        public int RequisitionId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public SubCategory? SubCategory { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
