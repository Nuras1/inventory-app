using System.ComponentModel.DataAnnotations.Schema;

namespace project_itransition.Models.Entities
{
    public class ItemFieldValue
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        public Guid InventoryFieldId { get; set; }

        public string? Value { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("InventoryFieldId")]
        public InventoryField? InventoryField { get; set; }
    }
}