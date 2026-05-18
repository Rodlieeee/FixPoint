using FixPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FixPoint.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();
            var userStatus = new Dictionary<string, bool>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);

                // IsActive = not locked out
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                userStatus[user.Id] = lockoutEnd == null ||
                                      lockoutEnd < DateTimeOffset.UtcNow;
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.UserStatus = userStatus;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Prevent changing the role of the system core administrator account
            if (user.Email != null && user.Email.Equals("admin@fixpoint.com", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "The system core administrator account role cannot be modified.";
                return RedirectToAction(nameof(Users));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Success"] = $"Role updated to {newRole} successfully!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Prevent admin from deactivating themselves / core admin account
            if (user.Email != null && user.Email.Equals("admin@fixpoint.com", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "You cannot deactivate the system admin!";
                return RedirectToAction(nameof(Users));
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            TempData["Success"] = $"{user.FullName} has been deactivated.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            TempData["Success"] = $"{user.FullName} has been reactivated.";
            return RedirectToAction(nameof(Users));
        }
    }
}