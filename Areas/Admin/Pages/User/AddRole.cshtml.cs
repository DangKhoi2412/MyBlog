// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TextTemplating;

namespace MyApp.Admin.Pages.User
{
    [Authorize(Roles = "Admin+Vip")]
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public AppUser user { get; set; }
        public string ReturnUrl { get; set; }
        [BindProperty]
        [DisplayName("Các role gán cho User")]
        public IList<string> RoleName { get; set; }
        public SelectList allRole { get; set; }

        public IList<IdentityRoleClaim<string>> ClaimInRole { get; set; }
        public IList<IdentityUserClaim<string>> ClaimInUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("Không có user");
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy User với Id {id}.");
            }
            RoleName = await _userManager.GetRolesAsync(user);
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRole = new SelectList(roleNames);
            await GetClaims(id);
            return Page();
        }
        private async Task GetClaims(string id)
        {
            // Sửa truy vấn cho ClaimInRole
            ClaimInRole = await _context.RoleClaims
                .Where(rc => _context.UserRoles
                    .Where(ur => ur.UserId == id)
                    .Select(ur => ur.RoleId)
                    .Contains(rc.RoleId))
                .ToListAsync();

            // Sửa truy vấn cho ClaimInUser - QUAN TRỌNG
            ClaimInUser = await _context.UserClaims
                .Where(uc => uc.UserId == id)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("Không có user");
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy User với Id {id}.");
            }
            var oldRoleName = await _userManager.GetRolesAsync(user);
            var deleteRole = oldRoleName.Where(r => !RoleName.Contains(r));
            var addRole = RoleName.Where(r => !oldRoleName.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRole = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRole);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRole);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Vừa cập nhật role cho {user.UserName}.";

            return RedirectToPage("./Index");
        }
    }
}
