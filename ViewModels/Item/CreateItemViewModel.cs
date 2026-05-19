namespace project_itransition.ViewModels.Item
{
    public class CreateItemViewModel
    {
        public Guid InventoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public uint xmin { get; set; }

        public List<Models.Entities.InventoryField> Fields { get; set; } = new List<Models.Entities.InventoryField>();

        public Dictionary<Guid, string> Values { get; set; } = new Dictionary<Guid, string>();
    }
}