using System.ComponentModel.DataAnnotations;

namespace project_itransition.Models.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
