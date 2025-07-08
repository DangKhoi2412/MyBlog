using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Admin.Pages.Roles;

[Authorize(Roles = "Admin,Admin+Vip")]
[Authorize(Policy = "AllowEditRole")]
public class EditModel : RolePageModel
{
    public EditModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : base(roleManager, dbContext)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Phải nhập tên của role")]
        [Display(Name = "Tên của role")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
        public string RoleName { get; set; }
    }
    public IdentityRole? Role { get; set; }
    public IList<IdentityRoleClaim<string>> Claims { get; set; }
    public async Task<IActionResult> OnGetAsync(string roleid)
    {
        if (roleid == null)
        {
            return NotFound("Không tìm thấy role");
        }
        Role = await _roleManager.FindByIdAsync(roleid);
        if (Role != null)
        {
            if (Role.Name == "Admin+Vip" && !User.IsInRole("Admin+Vip"))
            {
                return Redirect("/access-denied.html");
            }
            Input = new InputModel()
            {
                RoleName = Role.Name
            };
            Claims = await _dbContext.RoleClaims.Where(r => r.RoleId == roleid).ToListAsync();
            return Page();
        }
        return NotFound("Không tìm thấy role");
    }
    public async Task<IActionResult> OnPostAsync(string roleid)
    {
        if (roleid == null)
        {
            return NotFound("Không tìm thấy role");
        }
        Role = await _roleManager.FindByIdAsync(roleid);
        
        if (Role == null) return NotFound("Không tìm thấy role");

        if (Role.Name == "Admin+Vip" && !User.IsInRole("Admin+Vip"))
        {
            return Redirect("/access-denied.html");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Role.Name = Input.RoleName;
        var result = await _roleManager.UpdateAsync(Role);

        if (result.Succeeded)
        {
            // Redirect or show success message as needed
            StatusMessage = $"Bạn vừa cập nhật thành công role {Input.RoleName}";
            return RedirectToPage("./Index");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}

