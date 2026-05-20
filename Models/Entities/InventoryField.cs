 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_itransition.Models.Entities
{
    public class InventoryField
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string FieldType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }

        [Timestamp]
        public uint xmin { get; set; }
        public Guid InventoryId { get; set; }
        public bool ShowInTable { get; set; }
        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }
        public ICollection<ItemFieldValue> FieldValues { get; set; } = new List<ItemFieldValue>();
    }
}
