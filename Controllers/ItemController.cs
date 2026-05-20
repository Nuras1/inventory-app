using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Helpers;
using project_itransition.Models.Entities;
using project_itransition.Resources;
using project_itransition.ViewModels.Item;


namespace project_itransition.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        private async Task<bool> CanManageItems(Inventory inventory)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return false;
            }

            return inventory.IsPublic || inventory.OwnerId == user.Id || User.IsInRole("Admin") || inventory.AccessUsers.Any(u => u.UserId == user.Id);
        }

        [Authorize]
        public async Task<IActionResult> Create(Guid inventoryId)
        {
            var inventory = await context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return NotFound();
            }
            if (!await CanManageItems(inventory))
            {
                return Forbid();
            }

            var fields = await context.InventoryFields
                .Where(f => f.InventoryId == inventoryId)
                .OrderBy(f => f.Order)
                .ToListAsync();
            CreateItemViewModel model = new()
            {
                InventoryId = inventoryId,
                Fields = fields
            };
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            var inventory = await context.Inventories
                    .Include(i => i.Items)
                    .Include(i => i.AccessUsers)
                    .FirstOrDefaultAsync(i => i.Id == model.InventoryId);
            if (inventory == null)
            {
                return NotFound();
            }
            if (!await CanManageItems(inventory))
            {
                return Forbid();
            }

            var nextNumber = 1;

            var lastItem = inventory.Items
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefault();

            if (lastItem != null)
            {
                var parts = lastItem.CustomId.Split('-');
                if (int.TryParse(parts.Last(), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            var customId = CustomIdGenerator.Generate(inventory.CustomIdFormat, nextNumber);

            if (ModelState.IsValid)
            {

                Item item = new()
                {
                    Name = model.Name,
                    InventoryId = model.InventoryId,
                    CreatedAt = DateTime.UtcNow,
                    CustomId = customId
                };
                context.Items.Add(item);
                await context.SaveChangesAsync();
                foreach (var pair in model.Values)
                {
                    ItemFieldValue value = new()
                    {
                        ItemId = item.Id,
                        InventoryFieldId = pair.Key,
                        Value = pair.Value
                    };
                    context.ItemFieldValues.Add(value);
                }
                await context.SaveChangesAsync();

                TempData["Success"] = "Item created successfully";

                return RedirectToAction("Details", "Inventory", new { id = model.InventoryId });
            }
            model.Fields = await context.InventoryFields
                    .Where(f => f.InventoryId == model.InventoryId)
                    .OrderBy(f => f.Order)
                    .ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var item = await context.Items
                .Include(i => i.Inventory)
                .ThenInclude(i => i.AccessUsers)
                .Include(i => i.FieldValues)
                .ThenInclude(v => v.InventoryField)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();

            }
            var user = await userManager.GetUserAsync(User);

            bool canManageItems = false;

            if (user != null)
            {
                canManageItems = item.Inventory.IsPublic || item.Inventory.OwnerId == user.Id || User.IsInRole("Admin") || item.Inventory.AccessUsers.Any(a => a.UserId == user.Id);
            }
            ViewBag.CanManageItems = canManageItems;
            return View(item);
        }
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await context.Items
                .Include(i => i.Inventory)
                .ThenInclude(i => i.AccessUsers)
                .Include(i => i.FieldValues)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if (!await CanManageItems(item.Inventory))
            {
                return Forbid();
            }

            var fields = await context.InventoryFields
                .Where(f => f.InventoryId == item.InventoryId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            CreateItemViewModel model = new()
            {
                Name = item.Name,
                InventoryId = item.InventoryId,
                xmin = item.xmin,
                Fields = fields,
                Values = item.FieldValues
                    .ToDictionary(v => v.InventoryFieldId, v => v.Value)
            };
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, CreateItemViewModel model)
        {
            var item = await context.Items
                .Include(i => i.Inventory)
                .ThenInclude(i => i.AccessUsers)
                .Include(i => i.FieldValues)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if (!await CanManageItems(item.Inventory))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                item.Name = model.Name;

                foreach (var pair in model.Values)
                {
                    var existingValue = item.FieldValues
                        .FirstOrDefault(v =>
                        v.InventoryFieldId == pair.Key);

                    if (existingValue != null)
                    {
                        existingValue.Value = pair.Value;
                    }
                }
                context.Entry(item)
                        .Property("xmin")
                        .OriginalValue = model.xmin;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", @Resource.ThisItemWasModified);

                    model.Fields = await context.InventoryFields
                        .Where(f => f.InventoryId == item.InventoryId)
                        .OrderBy(f => f.Order)
                        .ToListAsync();

                    return View(model);
                }

                TempData["Success"] = @Resource.ItemUpdatedSuccessfully;

                return RedirectToAction("Details", "Inventory", new { id = item.InventoryId });
            }

            model.Fields = await context.InventoryFields
                .Where(f => f.InventoryId == item.InventoryId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await context.Items
                .Include(i => i.Inventory)
                .ThenInclude(i => i.AccessUsers)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if (!await CanManageItems(item.Inventory))
            {
                return Forbid();
            }

            return View(item);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var item = await context.Items
                .Include(i => i.Inventory)
                .ThenInclude(i => i.AccessUsers)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            if (!await CanManageItems(item.Inventory))
            {
                return Forbid();
            }

            var inventoryId = item.InventoryId;

            context.Items.Remove(item);

            await context.SaveChangesAsync();

            TempData["Success"] = @Resource.ItemDeletedSuccessfully;

            return RedirectToAction("Details", "Inventory", new { id = inventoryId });
        }

    }
}
