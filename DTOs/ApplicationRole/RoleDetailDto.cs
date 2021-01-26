namespace ImplicitManyToManyIdentityCore.DTOs.ApplicationRole
{
    public record RoleDetailDto
    {
        public int RoleId { get; init; }
        public int NumberOfUsers { get; init; }
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public string Description { get; init; }
        
    }
}
