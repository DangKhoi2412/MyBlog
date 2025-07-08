using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Core.Types;

namespace MyApp.Admin.Pages.User
{
    [Authorize(Roles = "Admin+Vip")]
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Phải nhập tên của {0}")]
            [Display(Name = "Tên Claim")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
            public string? ClaimType { get; set; }

            [Required(ErrorMessage = "Phải nhập tên của {0}")]
            [Display(Name = "Gía trị")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {2} đến {1} kí tự")]
            public string? ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public AppUser? user { set; get; }


        public async Task<IActionResult> OnGetAddClaim(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaim(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);
            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này có rồi");
                return Page();
            }
            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm Claim cho User";
            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
        public IdentityUserClaim<string>? userClaim { get; set; }
        public async Task<IActionResult> OnGetEditClaim(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy claim");

            // Tìm claim theo ID
            userClaim = _context.UserClaims.FirstOrDefault(c => c.Id == claimid);
            if (userClaim == null) return NotFound("Không tìm thấy claim");

            // Tìm user bằng UserId của claim (KHÔNG PHẢI claim.Id)
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");

            // Khởi tạo InputModel với giá trị hiện tại của claim
            Input = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditClaim(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy claim");
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (userClaim == null) return NotFound("Không tìm thấy claim");
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra claim trùng lặp
            var claims = _context.UserClaims
                .Where(c => c.UserId == user.Id && c.Id != claimid);

            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã tồn tại");
                return Page();
            }

            // Cập nhật claim
            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            _context.UserClaims.Update(userClaim);
            await _context.SaveChangesAsync();

            StatusMessage = "Đã cập nhật Claim cho User";
            return RedirectToPage("./AddRole", new { Id = userClaim.UserId });
        }
        public NotFoundObjectResult OnGet() { return NotFound("Không được truy cập"); }
        public async Task<IActionResult> OnPostDelete(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy claim");
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (userClaim == null) return NotFound("Không tìm thấy claim");
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");

            await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            StatusMessage = "Ban da xoa";
            return RedirectToPage("./AddRole", new { Id = userClaim.UserId });
        }
    }
}
