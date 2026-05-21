using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;
using project_itransition.Resources;
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
            var user = await GetCurrentUserAsync();

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

            var user = await GetCurrentUserAsync();

            if (!HasInventoryAccess(inventory, user))
            {
                TempData["Error"] = @Resource.AccessDenied;
                return RedirectToAction(nameof(Index));
            }

            bool canEdit = user != null && ( inventory.OwnerId == user.Id|| IsAdmin());

            ViewBag.CanEdit = canEdit;

            bool canManageItems = false;

            if (user != null)
            {
                canManageItems = inventory.IsPublic || inventory.OwnerId == user.Id || IsAdmin() || inventory.AccessUsers.Any(a => a.UserId == user.Id);
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

            var user = await GetCurrentUserAsync();

            if (!CanEditInventory(inventory, user))
            {
                return Forbid();
            }
            ViewBag.CanEdit = true;

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
            var user = await GetCurrentUserAsync();

            if (!CanEditInventory(inventoryToUpdate, user))
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
                ModelState.AddModelError("", @Resource.ThisInventoryWasModified);

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
            var user = await GetCurrentUserAsync();

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

            var user = await GetCurrentUserAsync();

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
                var user = await GetCurrentUserAsync();
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
                    await AddTagsToInventoryAsync(inventory, model.Tags);
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
        private bool CanEditInventory(Inventory inventory, ApplicationUser? user)
        {
            return user != null && (inventory.OwnerId == user.Id || IsAdmin());
        }
        [HttpGet]
        public async Task<IActionResult> SearchTags(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<string>());
            }

            var tags = await context.Tags
                .Where(t => t.Name.StartsWith(term.ToLower()))
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .Take(8)
                .ToListAsync();

            return Json(tags);
        }
        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return await userManager.GetUserAsync(User);
        }
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }
        private bool HasInventoryAccess(Inventory inventory, ApplicationUser? user)
        {
            if (inventory.IsPublic)
            {
                return true;
            }

            if (user == null)
            {
                return false;
            }

            return inventory.OwnerId == user.Id || IsAdmin() || inventory.AccessUsers.Any(a => a.UserId == user.Id);
        }
        private async Task AddTagsToInventoryAsync(Inventory inventory, string tags)
        {
            var tagNames = tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
    }
}
