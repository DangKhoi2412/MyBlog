using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApp.Admin.Pages.Roles;

[Authorize(Roles ="Admin+Vip")]
public class EditRoleClaimModel : RolePageModel
{
    public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : base(roleManager, dbContext)
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
    public IdentityRoleClaim<string>? claim { get; set; }


    public async Task<IActionResult> OnGet(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy Cliam");
        claim = await _dbContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefaultAsync();
        if (claim == null) return NotFound("Không tìm thấy Claim");
        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound("Không tìm thấy role");
        Input = new InputModel()
        {
            ClaimType = claim.ClaimType,
            ClaimValue = claim.ClaimValue
        };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy Cliam");
        claim = await _dbContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefaultAsync();
        if (claim == null) return NotFound("Không tìm thấy Claim");
        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound("Không tìm thấy role");

        if (!ModelState.IsValid) return Page();
        var IsExictClaim = _dbContext.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType
                                                        && c.ClaimValue == Input.ClaimValue
                                                        && c.Id != claim.Id);
        if (IsExictClaim)
        {
            ModelState.AddModelError(string.Empty, "Cliams này đã có");
            return Page();
        }
        claim.ClaimType = Input.ClaimType;
        claim.ClaimValue = Input.ClaimValue;
        var result = await _dbContext.SaveChangesAsync();

        StatusMessage = $"Vừa thêm cliam {claim.ClaimType}";
        return RedirectToPage("./Edit", new { roleid = role.Id });
    }


    public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
    {
        if (claimid == null) return NotFound("Không tìm thấy Cliam");
        claim = await _dbContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefaultAsync();
        if (claim == null) return NotFound("Không tìm thấy Claim");
        role = await _roleManager.FindByIdAsync(claim.RoleId);
        if (role == null) return NotFound("Không tìm thấy role");

        await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
        StatusMessage = $"Vừa xóa Claim {claim.ClaimType}";
        return RedirectToPage("./Edit", new { roleid = role.Id });
    }
}

