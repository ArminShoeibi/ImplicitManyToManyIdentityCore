namespace ImplicitManyToManyIdentityCore.DTOs.ApplicationRole
{
    public record UpdateApplicationRoleDto : CreateApplicationRoleDto
    {
        public int RoleId { get; init; }
    }
}
