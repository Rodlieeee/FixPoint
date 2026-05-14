using FixPoint.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FixPoint.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // One-to-one: MaintenanceRequest ↔ Assignment
            builder.Entity<Assignment>()
                .HasOne(a => a.MaintenanceRequest)
                .WithOne(r => r.Assignment)
                .HasForeignKey<Assignment>(a => a.MaintenanceRequestId);

            // MaintenanceRequest → ReportedBy User
            builder.Entity<MaintenanceRequest>()
                .HasOne(r => r.ReportedBy)
                .WithMany(u => u.ReportedRequests)
                .HasForeignKey(r => r.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment → Technician User
            builder.Entity<Assignment>()
                .HasOne(a => a.Technician)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}