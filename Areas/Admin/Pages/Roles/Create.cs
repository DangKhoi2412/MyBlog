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
public class CreateModel : RolePageModel
{
    public CreateModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : base(roleManager, dbContext)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public void OnGet()
    {

    }
    public class InputModel
    {
        [Required(ErrorMessage = "Phải nhập tên của role")]
        [Display(Name = "Tên của role")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
        public string RoleName { get; set; }
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var role = new IdentityRole(Input.RoleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            // Redirect or show success message as needed
            StatusMessage = $"Bạn vừa tạo thành công role {Input.RoleName}";
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

