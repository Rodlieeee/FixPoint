using FixPoint.Data;
using FixPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixPoint.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var facilities = await _context.Facilities.ToListAsync();
            return View(facilities);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Facility facility)
        {
            if (ModelState.IsValid)
            {
                _context.Facilities.Add(facility);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Facility created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null) return NotFound();
            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facility facility)
        {
            if (id != facility.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(facility);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Facility updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var facility = await _context.Facilities
                .Include(f => f.MaintenanceRequests)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (facility == null) return NotFound();
            return View(facility);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility != null)
            {
                _context.Facilities.Remove(facility);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Facility deleted!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}