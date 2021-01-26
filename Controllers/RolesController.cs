using ImplicitManyToManyIdentityCore.Data;
using ImplicitManyToManyIdentityCore.Domain.Identity;
using ImplicitManyToManyIdentityCore.DTOs.ApplicationRole;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImplicitManyToManyIdentityCore.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            List<RoleDetailDto> roles = await _roleManager.Roles
                .Select(r => new RoleDetailDto
                {
                    Description = r.Description,
                    DisplayName = r.DisplayName,
                    Name = r.Name,
                    RoleId = r.Id,
                    NumberOfUsers = r.Users.Count(), // using Skip Navigation
                }).ToListAsync();

            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateApplicationRoleDto dto)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole newApplicationRole = new()
                {
                    Description = dto.Description,
                    DisplayName = dto.DisplayName,
                    Name = dto.Name,
                };

                IdentityResult createRoleResult = await _roleManager.CreateAsync(newApplicationRole);
                if (createRoleResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in createRoleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            return View(dto);
        }

        public async Task<IActionResult> EditRole(int id)
        {
            UpdateApplicationRoleDto applicationRoleToUpdate = await _roleManager.Roles.Where(r => r.Id == id)
                 .Select(r => new UpdateApplicationRoleDto
                 {
                     Description = r.Description,
                     DisplayName = r.DisplayName,
                     Name = r.Name,
                     RoleId = r.Id
                 })
                 .FirstOrDefaultAsync();


            if (applicationRoleToUpdate is null)
            {
                ViewBag.ErrorMessage = "سطح دسترسی مورد نظر وجود ندارد.";
                return View("NotFound");
            }

            return View(applicationRoleToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UpdateApplicationRoleDto dto, [FromServices] ApplicationContext _db)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole applicationRoleToUpdate = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
                if (applicationRoleToUpdate is null)
                {
                    return NotFound();
                }
                applicationRoleToUpdate.Name = dto.Name;
                applicationRoleToUpdate.DisplayName = dto.DisplayName;
                applicationRoleToUpdate.Description = dto.Description;
                applicationRoleToUpdate.ConcurrencyStamp = Guid.NewGuid().ToString();

                await _roleManager.UpdateNormalizedRoleNameAsync(applicationRoleToUpdate);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(dto);
        }
    }
}
