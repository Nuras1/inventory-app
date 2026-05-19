using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;
using project_itransition.ViewModels.Inventory;

namespace project_itransition.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public InventoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var inventories = await context.Inventories
                .Include(i => i.Owner)
                .ToListAsync();

            return View(inventories);
        }
        public async Task<IActionResult> MyInventories()
        {
            var user = await userManager.GetUserAsync(User);

            var inventories = await context.Inventories
                .Include(i => i.Owner)
                .Where(i => i.OwnerId == user.Id)
                .ToListAsync();
            return View(inventories);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var inventory = await context.Inventories
                .Include(i => i.Owner)
                .Include(i => i.Tags)
                .Include(i => i.Items)
                .ThenInclude(item => item.FieldValues)
                .Include(i => i.InventoryFields)
                .Include(i => i.AccessUsers)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }
            var user = await userManager.GetUserAsync(User);

            bool canEdit = user != null &&
                (
                    inventory.OwnerId == user.Id || User.IsInRole("Admin")
                );

            ViewBag.CanEdit = canEdit;

            bool canManageItems = false;

            if (user != null)
            {
                canManageItems = inventory.IsPublic || inventory.OwnerId == user.Id || User.IsInRole("Admin") || inventory.AccessUsers.Any(a => a.UserId == user.Id);
            }

            ViewBag.CanManageItems = canManageItems;

            return View(inventory);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var inventory = await context.Inventories
                .Include(i => i.AccessUsers)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);

            if (!CanEditInventory(inventory, user))
            {
                return Forbid();
            }

            return View(inventory);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Inventory inventory)
        {
            var inventoryToUpdate = await context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventory.Id);

            if (inventoryToUpdate == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);

            if (!CanEditInventory(inventory, user))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(inventory);
            }

            inventoryToUpdate.Title = inventory.Title;
            inventoryToUpdate.Description = inventory.Description;
            inventoryToUpdate.Category = inventory.Category;
            inventoryToUpdate.Prefix = inventory.Prefix;
            inventoryToUpdate.CustomIdFormat = inventory.CustomIdFormat;
            inventoryToUpdate.IsPublic = inventory.IsPublic;

            context.Entry(inventoryToUpdate)
                        .Property("xmin")
                        .OriginalValue = inventory.xmin;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "This inventory was modified by another user. Reload the page and try again.");

                return View(inventory);
            }

            return RedirectToAction("MyInventories");
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var inventory = await context.Inventories
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }
            var user = await userManager.GetUserAsync(User);

            if (!CanEditInventory(inventory, user))
            {
                return Forbid();
            }

            return View(inventory);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inventory = await context.Inventories
                        .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);

            if (!CanEditInventory(inventory, user))
            {
                return Forbid();
            }

            context.Inventories.Remove(inventory);

            await context.SaveChangesAsync();

            return RedirectToAction("MyInventories");
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateInventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                Inventory inventory = new Inventory
                {
                    Prefix = model.Prefix,
                    Title = model.Title,
                    Description = model.Description,
                    Category = model.Category,
                    CustomIdFormat = model.CustomIdFormat,
                    IsPublic = model.IsPublic,
                    OwnerId = user!.Id
                };
                if (!string.IsNullOrWhiteSpace(model.Tags))
                {
                    var tagNames = model.Tags
                        .Split(',')
                        .Select(t => t.Trim().ToLower())
                        .Distinct();

                    foreach (var tagName in tagNames)
                    {
                        var existingTag = await context.Tags
                            .FirstOrDefaultAsync(t => t.Name == tagName);

                        if (existingTag == null)
                        {
                            existingTag = new Tag
                            {
                                Name = tagName
                            };

                            context.Tags.Add(existingTag);
                        }

                        inventory.Tags.Add(existingTag);
                    }
                }
                context.Inventories.Add(inventory);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }
            var inventories = await context.Inventories
                .Include(i => i.Owner)
                .Include(i => i.Tags)
                .Where(i =>
                i.Title.Contains(query) ||
                i.Description.Contains(query) ||
                i.Category.Contains(query) ||
                i.Tags.Any(t => t.Name.Contains(query)))
                .ToListAsync();
            ViewBag.Query = query;
            return View(inventories);
        }
        public async Task<IActionResult> ByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return RedirectToAction("Index");
            }
            var inventories = await context.Inventories
                .Include(i => i.Owner)
                .Include(i => i.Tags)
                .Where(i => i.Tags.Any(t => t.Name == tag))
                .ToListAsync();
            ViewBag.Tag = tag;
            return View(inventories);
        }
        private bool CanEditInventory(Inventory inventory,ApplicationUser? user)
        {
            return user != null &&(inventory.OwnerId == user.Id || User.IsInRole("Admin"));
        }
    }
}
