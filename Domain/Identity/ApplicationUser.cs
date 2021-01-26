using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ImplicitManyToManyIdentityCore.Domain.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<IdentityUserRole<int>> UserRoles { get; set; }
        public ICollection<ApplicationRole> Roles { get; set; }
    }
}
