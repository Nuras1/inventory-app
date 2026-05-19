namespace project_itransition.Models.Entities
{
    public class InventoryAccess
    {
        public int Id  { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User{ get; set; }
    }
}
