using System.ComponentModel.DataAnnotations;

namespace project_itransition.ViewModels.InventoryField
{
    public class CreateFieldViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string FieldType { get; set; } = "text";
        public string? Description { get; set; }
        public bool ShowInTable { get; set; }
        public Guid InventoryId { get; set; }
    }
}
