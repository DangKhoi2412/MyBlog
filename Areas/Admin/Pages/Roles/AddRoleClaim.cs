using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApp.Admin.Pages.Roles;

public class AddRoleClaimModel : RolePageModel
{
    public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : base(roleManager, dbContext)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Phải nhập tên của {0}")]
        [Display(Name = "Tên Claim")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Phải nhập tên của {0}")]
        [Display(Name = "Gía trị")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
        public string ClaimValue { get; set; } 
    }
    public IdentityRole? role { get; set; }
    public async Task<IActionResult> OnGet(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (role == null) return NotFound("Không tìm thấy role");
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        role = await _roleManager.FindByIdAsync(roleid);
        if (role == null) return NotFound("Không tìm thấy role");
        if (!ModelState.IsValid) return Page();
        var IsExictClaim = (await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue);
        if (IsExictClaim)
        {
            ModelState.AddModelError(string.Empty, "Cliams này đã có");
            return Page();
        }
        var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
        var result = await _roleManager.AddClaimAsync(role, newClaim);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        StatusMessage = $"Vừa thêm cliam {newClaim.Type}";
        return RedirectToPage("./Edit", new {roleid = role.Id});
    }
}

