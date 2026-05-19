using Microsoft.AspNetCore.Identity;

namespace project_itransition.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsBlocked { get; set; }
    }
}
