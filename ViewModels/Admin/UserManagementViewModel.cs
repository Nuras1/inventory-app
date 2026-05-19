namespace project_itransition.ViewModels.Admin
{
    public class UserManagementViewModel
    {
        public string Id{ get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
    }
}
