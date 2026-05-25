using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_itransition.Models.Entities;
using project_itransition.ViewModels.Admin;

namespace project_itransition.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        public AdminController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users
                .ToListAsync();
            var model = new List<UserManagementViewModel>();
            foreach (var user in users)
            {
                model.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsBlocked = user.IsBlocked,
                    IsAdmin = await userManager.IsInRoleAsync(user, "Admin")
                });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Block(UserManagementViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.IsBlocked = true;
                user.LockoutEnabled = true;
                var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1000));
                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UnBlock(UserManagementViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.IsBlocked = false;

                var result = await userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UserManagementViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(UserManagementViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Admin");
                
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(UserManagementViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }
    }
}
