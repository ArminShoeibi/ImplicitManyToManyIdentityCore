using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ImplicitManyToManyIdentityCore.Domain.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public ICollection<IdentityUserRole<int>> UserRoles { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
