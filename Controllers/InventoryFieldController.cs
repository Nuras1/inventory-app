using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;
using project_itransition.ViewModels.InventoryField;

namespace project_itransition.Controllers
{
    [Authorize]
    public class InventoryFieldController : Controller

    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public InventoryFieldController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        private async Task<bool> CanEditInventory(Guid inventoryId)
        {
            var inventory = await context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return false;
            }

            var user = await userManager.GetUserAsync(User);

            return user != null && (inventory.OwnerId == user.Id || IsAdmin());
        }
        public async Task<IActionResult> Create(Guid inventoryId)
        {
            if (!await CanEditInventory(inventoryId))
            {
                return Forbid();
            }

            return View(new CreateFieldViewModel
            {
                InventoryId = inventoryId
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateFieldViewModel model)
        {

            if (!await CanEditInventory(model.InventoryId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await context.InventoryFields
                .CountAsync(f => f.InventoryId == model.InventoryId);

            InventoryField field = new()
            {
                Name = model.Name,
                FieldType = model.FieldType,
                Description = model.Description,
                ShowInTable = model.ShowInTable,
                InventoryId = model.InventoryId,
                Order = order + 1
            };

            context.InventoryFields.Add(field);

            await context.SaveChangesAsync();

            return RedirectToAction("Details", "Inventory", new { id = model.InventoryId });

        }
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}
