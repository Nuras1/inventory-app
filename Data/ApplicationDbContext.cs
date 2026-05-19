using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project_itransition.Models.Entities;

namespace project_itransition.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<InventoryField> InventoryFields { get; set; }

        public DbSet<ItemFieldValue> ItemFieldValues { get; set; }

        public DbSet<CustomIdPart> CustomIdParts { get; set; }

        public DbSet<InventoryAccess> InventoryAccesses { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inventory>()
                .HasMany(i => i.Tags)
                .WithMany(t => t.Inventories);

            builder.Entity<Item>()
                .HasIndex(i => new
                {
                    i.InventoryId, i.CustomId
                })
                .IsUnique();
        }
    }
}