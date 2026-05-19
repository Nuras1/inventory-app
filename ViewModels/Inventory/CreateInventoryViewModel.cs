using System.ComponentModel.DataAnnotations;

namespace project_itransition.ViewModels.Inventory
{
    public class CreateInventoryViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Desctiption is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Prefix is required")]
        public string Prefix { get; set; } = "INV";
        public uint xmin { get; set; }
        public string? CustomIdFormat { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "Other";
        public bool IsPublic { get; set; }

        public string? Tags { get; set; }
    }
}
