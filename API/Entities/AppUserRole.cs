using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // this is the joining table for roles and users many to many 
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}