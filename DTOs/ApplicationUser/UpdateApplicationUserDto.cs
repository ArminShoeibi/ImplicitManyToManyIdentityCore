using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ImplicitManyToManyIdentityCore.DTOs.ApplicationUser
{
    public record UpdateApplicationUserDto
    {
        [HiddenInput]
        public int ApplicationUserId { get; set; }

        [Required]
        [StringLength(40)]
        [Display(Name = "First Name")]
        public string FirstName { get; init; }


        [Required]
        [StringLength(40)]
        [Display(Name = "Last Name")]
        public string LastName { get; init; }

        [Required]
        [StringLength(40)]
        public string UserName { get; init; }

        [EmailAddress]
        [Required]
        public string Email { get; init; }


        [Required]
        [StringLength(11, MinimumLength = 8)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; init; }


        public string[] Roles { get; set; }
    }
}
