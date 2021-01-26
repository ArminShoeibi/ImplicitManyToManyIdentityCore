using ImplicitManyToManyIdentityCore.Data;
using ImplicitManyToManyIdentityCore.Domain.Identity;
using ImplicitManyToManyIdentityCore.DTOs.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImplicitManyToManyIdentityCore.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [NonAction]
        public async Task GetRolesSelectList()
        {
            var roles = await _db.Roles
            .Select(r => new { r.Name, r.DisplayName })
            .ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Name", "DisplayName");
        }
        
        public async Task<IActionResult> Index()
        {
            List<ApplicationUserDetailDto> users = await _db.Users
                .Select(u => new ApplicationUserDetailDto
                {
                    ApplicationUserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = $"{u.FirstName} {u.LastName}",
                    PhoneNumber = u.PhoneNumber,
                    Roles = string.Join(" - ",u.Roles.Select(r=> r.DisplayName))
                })
                .ToListAsync();

            return View(users);
        }

        
        public async Task<IActionResult> CreateUser()
        {

            await GetRolesSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateApplicationUserDto dto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newApplicationUser = new()
                {
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    UserName = dto.UserName,
                    PhoneNumber = dto.PhoneNumber,

                };

                if (dto.Roles is not null)
                {
                    var roles = await _db.Roles.Where(r => dto.Roles.Contains(r.Name))
                                      .ToListAsync();

                    newApplicationUser.Roles = roles; // Using Skip Navigation
                }

                IdentityResult createUserResult = await _userManager.CreateAsync(newApplicationUser, dto.Password);
                if (createUserResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await GetRolesSelectList();
                    return View(dto);
                }

            }

            await GetRolesSelectList();
            return View(dto);
        }


        public async Task<IActionResult> EditUser(int id)
        {
            var applicationUserToUpdate = await _db.Users.Where(u => u.Id == id)
                .Select(u => new UpdateApplicationUserDto
                {
                    ApplicationUserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Roles = u.Roles.Select(r => r.Name).ToArray(), // Skip Navigation (EF Core 5)
                    UserName = u.UserName,
                })
                .FirstOrDefaultAsync();

            if (applicationUserToUpdate is null)
            {
                return NotFound();
            }

            await GetRolesSelectList();
            return View(applicationUserToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UpdateApplicationUserDto dto)
        {
            if (ModelState.IsValid)
            {
               ApplicationUser applicationUserToUpdate = await _db.Users.Where(u => u.Id == dto.ApplicationUserId)
                    .Include(u => u.Roles) /* Really Important */
                    .FirstOrDefaultAsync();

                if (applicationUserToUpdate is null)
                {
                    return NotFound();
                }

                /* Easy :) */
                List<ApplicationRole> selectedRoles = await _db.Roles.Where(r => dto.Roles.Contains(r.Name)).ToListAsync();
                applicationUserToUpdate.Roles = selectedRoles;
                /* Easy :) */

                applicationUserToUpdate.FirstName = dto.FirstName;
                applicationUserToUpdate.LastName = dto.LastName;
                applicationUserToUpdate.Email = dto.Email;
                applicationUserToUpdate.PhoneNumber = dto.PhoneNumber;
                applicationUserToUpdate.ConcurrencyStamp = Guid.NewGuid().ToString();

                await _userManager.UpdateNormalizedEmailAsync(applicationUserToUpdate); // deferred
                await _userManager.UpdateNormalizedUserNameAsync(applicationUserToUpdate); // deferred
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await GetRolesSelectList();
            return View(dto);
        }
    }
}
