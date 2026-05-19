using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;
using System.Security.Claims;

namespace project_itransition.Helpers
{
    public static class InventoryPermissionHelper
    {
        public static async Task<bool> CanViewInventory(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ClaimsPrincipal user, Inventory inventory)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return false;
            }
            var currentUser = await userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                return false;
            }
            if (await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return true;
            }
            if (inventory.OwnerId == currentUser.Id)
            {
                return true;
            }
            if (inventory.IsPublic)
            {
                return true;
            }
            return await context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == inventory.Id && a.UserId == currentUser.Id);
        }
    }
}
