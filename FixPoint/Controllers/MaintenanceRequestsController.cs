using FixPoint.Data;
using FixPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FixPoint.Controllers
{
    [Authorize]
    public class MaintenanceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaintenanceRequestController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            var requests = isAdmin
                ? await _context.MaintenanceRequests
                    .Include(r => r.Facility)
                    .Include(r => r.ReportedBy)
                    .Include(r => r.Assignment)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync()
                : await _context.MaintenanceRequests
                    .Include(r => r.Facility)
                    .Include(r => r.Assignment)
                    .Where(r => r.ReportedById == user.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.MaintenanceRequests
                .Include(r => r.Facility)
                .Include(r => r.ReportedBy)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a.Technician)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();
            return View(request);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Facilities = new SelectList(
                await _context.Facilities.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            request.ReportedById = user.Id;
            request.CreatedAt = DateTime.Now;
            request.Status = RequestStatus.Open;

            ModelState.Remove("ReportedById");
            ModelState.Remove("ReportedBy");
            ModelState.Remove("Facility");

            if (ModelState.IsValid)
            {
                _context.MaintenanceRequests.Add(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Request submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Facilities = new SelectList(
                await _context.Facilities.ToListAsync(), "Id", "Name");
            return View(request);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(int id)
        {
            var request = await _context.MaintenanceRequests
                .Include(r => r.Facility)
                .Include(r => r.Assignment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            var technicians = await _userManager.GetUsersInRoleAsync("Technician");
            ViewBag.Technicians = new SelectList(technicians, "Id", "FullName");

            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, string technicianId, string notes)
        {
            var request = await _context.MaintenanceRequests
                .Include(r => r.Assignment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            if (request.Assignment == null)
            {
                var assignment = new Assignment
                {
                    MaintenanceRequestId = id,
                    TechnicianId = technicianId,
                    Notes = notes,
                    AssignedAt = DateTime.Now
                };
                _context.Assignments.Add(assignment);
            }
            else
            {
                request.Assignment.TechnicianId = technicianId;
                request.Assignment.Notes = notes;
                request.Assignment.AssignedAt = DateTime.Now;
            }

            request.Status = RequestStatus.InProgress;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Technician assigned successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, RequestStatus status)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Status updated!";
            return RedirectToAction(nameof(Index));
        }
    }
}