using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_itransition.Models.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; } = "Other";

        public string Prefix { get; set; } = "INV";

        public int NextSequence { get; set; } = 1;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; } = string.Empty;
        public string? CustomIdFormat { get; set; }

        [Timestamp]
        public uint xmin { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();

        public ICollection<InventoryField> InventoryFields { get; set; } = new List<InventoryField>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<InventoryAccess> AccessUsers { get; set; } = new List<InventoryAccess>();
    }
}
