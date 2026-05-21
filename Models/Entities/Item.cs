using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_itransition.Models.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? CustomId { get; set; } = string.Empty;

        [Timestamp]
        public uint xmin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public Inventory? Inventory { get; set; }
        public ICollection<ItemFieldValue> FieldValues { get; set; }
            = new List<ItemFieldValue>();
    }
}