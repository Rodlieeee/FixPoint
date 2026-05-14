using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixPoint.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        // FK → MaintenanceRequest (one-to-one)
        public int MaintenanceRequestId { get; set; }
        public MaintenanceRequest MaintenanceRequest { get; set; }

        // FK → Technician (ApplicationUser)
        [Required]
        public string TechnicianId { get; set; }
        public ApplicationUser Technician { get; set; }
    }
}