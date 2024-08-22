using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlow.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }

        [Required]

        [MaxLength(25)]
        public string Code { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [MaxLength(10)]
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(10)]
        public string? InactivatedBy { get; set; }
    }
}
