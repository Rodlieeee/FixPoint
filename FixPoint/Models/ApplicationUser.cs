using Microsoft.AspNetCore.Identity;

namespace FixPoint.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Navigation
        public ICollection<MaintenanceRequest> ReportedRequests { get; set; }
            = new List<MaintenanceRequest>();

        public ICollection<Assignment> Assignments { get; set; }
            = new List<Assignment>();
    }
}