using System.ComponentModel.DataAnnotations;

namespace FixPoint.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        // Technician feedback
        public string? FeedbackNotes { get; set; }
        public string? ProofPhotoPath { get; set; }
        public DateTime? FeedbackSubmittedAt { get; set; }

        // FK → MaintenanceRequest
        public int MaintenanceRequestId { get; set; }
        public MaintenanceRequest MaintenanceRequest { get; set; }

        // FK → Technician
        [Required]
        public string TechnicianId { get; set; }
        public ApplicationUser Technician { get; set; }
    }
}