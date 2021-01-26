using System.ComponentModel.DataAnnotations;

namespace ImplicitManyToManyIdentityCore.DTOs.ApplicationRole
{
    public record CreateApplicationRoleDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; }

        [Required]
        [StringLength(80)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; init; }

        [StringLength(200)]
        public string Description { get; init; }
    }
}
