using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixPoint.Models
{
    public enum RequestStatus { Open, InProgress, Resolved, Closed }
    public enum PriorityLevel { Low, Medium, High, Critical }

    public class MaintenanceRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Open;
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK → Facility
        [Required]
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }

        // FK → User who reported it
        [Required]
        public string ReportedById { get; set; }
        public ApplicationUser ReportedBy { get; set; }

        // Navigation to assignment
        public Assignment? Assignment { get; set; }
    }
}