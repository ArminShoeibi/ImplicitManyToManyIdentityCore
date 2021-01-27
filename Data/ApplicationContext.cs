using ImplicitManyToManyIdentityCore.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImplicitManyToManyIdentityCore.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                        .HasMany(u => u.Roles)
                        .WithMany(r => r.Users)
                        .UsingEntity<IdentityUserRole<int>>
                        (au => au.HasOne<ApplicationRole>().WithMany(role => role.UserRoles).HasForeignKey(role=> role.RoleId),
                        au => au.HasOne<ApplicationUser>().WithMany(user => user.UserRoles).HasForeignKey(user=> user.UserId));

        }
    }
}
