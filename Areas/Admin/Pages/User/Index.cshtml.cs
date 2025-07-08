using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Admin.Pages.User
{
    [Authorize(Roles ="Admin,Admin+Vip")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class UserAndRole : AppUser
        {
            public string RoleName { get; set; }
        }
        public List<UserAndRole> users { set; get; }
        public string? Title { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public const int ITEM_PER_PAGE = 15;

        [BindProperty(SupportsGet = true, Name = "trang")]
        public int currentPage { set; get; }
        public int countPages { get; set; }
        public int totalUser { get; set; }

        public async Task OnGet()
        {
            // users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var query = _userManager.Users.OrderBy(u => u.UserName);
            totalUser = await query.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUser / ITEM_PER_PAGE);
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;


            var qr1 = query.Skip((currentPage - 1) * 10)
                        .Take(ITEM_PER_PAGE)
                        .Select(u => new UserAndRole()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                        });
            users = await qr1.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleName = string.Join(", ", roles);
            }
        }
        public void OnPost()
        {
            RedirectToPage();
        }
    }
}
