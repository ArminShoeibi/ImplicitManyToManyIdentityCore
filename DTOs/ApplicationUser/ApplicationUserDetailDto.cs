namespace ImplicitManyToManyIdentityCore.DTOs.ApplicationUser
{
    public record ApplicationUserDetailDto
    {
        public int ApplicationUserId { get; set; }
        public string FullName { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string Roles { get; set; }
    }
}
