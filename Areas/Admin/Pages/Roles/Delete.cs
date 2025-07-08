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
public class DeleteModel : RolePageModel
{
    public DeleteModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : base(roleManager, dbContext)
    {
    }

    [BindProperty]
    public IdentityRole? Role { get; set; }
    public async Task<IActionResult> OnGetAsync(string roleid)
    {
        if (roleid == null)
        {
            return NotFound("Không tìm thấy role");
        }
        Role = await _roleManager.FindByIdAsync(roleid);
        if (Role == null)
        {
            return NotFound("Không tìm thấy role");
        }
        if (Role.Name == "Admin+Vip" && !User.IsInRole("Admin+Vip"))
        {
            return Redirect("/access-denied.html");
        }
        return Page();
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
        var result = await _roleManager.DeleteAsync(Role); 

        if (result.Succeeded)
        {
            // Redirect or show success message as needed
            StatusMessage = $"Bạn vừa cập nhật thành công role {Role.Name}";
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

