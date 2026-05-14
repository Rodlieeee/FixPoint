using System.ComponentModel.DataAnnotations;

namespace FixPoint.Models
{
    public class Facility
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        public string? Description { get; set; }

        // Navigation
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
            = new List<MaintenanceRequest>();
    }
}