using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WorkFlow.Models;

namespace WorkFlow.Data.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<LastUserId> LastUserIds { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<RequisitionHeader> RequisitionHeaders { get; set; }
        public DbSet<RequisitionBody> RequisitionBodies { get; set; }
        public DbSet<RequisitionApproval> RequisitionApprovals { get; set; }
        public DbSet<RequisitionSupplement> RequisitionSupplements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define relationships and default values
            builder.Entity<Category>()
            .HasIndex(c => c.Code)
            .IsUnique();

            builder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(sc => sc.Category)
                .HasForeignKey(sc => sc.CategoryId);

            builder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Category>()
                .Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Entity<SubCategory>()
                .Property(sc => sc.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<SubCategory>()
                .Property(sc => sc.IsActive)
                .HasDefaultValue(true);
            // Configure RequisitionHeader entity
            builder.Entity<RequisitionHeader>()
                .HasKey(rh => rh.RequisitionId);

            builder.Entity<RequisitionHeader>()
                .HasOne(rh => rh.Category)
                .WithMany() // Adjust if there is a navigation property in Category
                .HasForeignKey(rh => rh.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Disabling cascading delete

            builder.Entity<RequisitionHeader>()
                .HasOne(rh => rh.SubCategory)
                .WithMany() // Adjust if there is a navigation property in SubCategory
                .HasForeignKey(rh => rh.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Disabling cascading delete

            // Configure RequisitionBody entity
            builder.Entity<RequisitionBody>()
                .HasKey(rb => rb.RequisitionId);

            builder.Entity<RequisitionBody>()
                .HasOne(rb => rb.RequisitionHeader)
                .WithMany() // Adjust if there is a navigation property in RequisitionHeader
                .HasForeignKey(rb => rb.RequisitionId);

            // Configure RequisitionApproval entity
            builder.Entity<RequisitionApproval>()
                 .HasKey(ra => ra.ApprovalId);

            builder.Entity<RequisitionApproval>()
                .Property(ra => ra.ApprovalId)
                .ValueGeneratedOnAdd(); // This makes ApprovalId an identity column

            builder.Entity<RequisitionApproval>()
                .HasOne(ra => ra.RequisitionHeader)
                .WithMany() // Adjust if there is a navigation property in RequisitionHeader
                .HasForeignKey(ra => ra.RequisitionId);

            builder.Entity<RequisitionSupplement>()
                  .HasKey(rs => rs.SupplementId);

            builder.Entity<RequisitionSupplement>()
                .Property(rs => rs.SupplementId)
                .ValueGeneratedOnAdd(); // This makes SupplementId an identity column

            builder.Entity<RequisitionSupplement>()
                .HasOne(rs => rs.RequisitionHeader)
                .WithMany() // Adjust if there is a navigation property in RequisitionHeader
                .HasForeignKey(rs => rs.RequisitionId);

        }
    }

    public class LastUserId
    {
        public int Id { get; set; }
        public int LastId { get; set; }
    }
}
