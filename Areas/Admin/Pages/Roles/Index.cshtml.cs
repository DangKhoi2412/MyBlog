using System.Threading.Tasks;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Admin.Pages.Roles
{
    [Authorize(Roles = "Admin,Admin+Vip")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> userManager, AppDbContext dbContext) : base(userManager, dbContext)
        {
        }
        public class RoleModel : IdentityRole
        {
            public IList<string> Claims { get; set; }
        }
        public IList<RoleModel> roles { set; get; }
        public string? Title { get; set; }
        public async Task OnGet()
        {
            var r = await _roleManager.Roles.ToListAsync();
            roles = new List<RoleModel>();
            foreach (var item in r)
            {
                var cliams = await _roleManager.GetClaimsAsync(item);
                var roleModel = new RoleModel()
                {
                    Name = item.Name,
                    Id = item.Id,
                    Claims = cliams.Select(c => c.Type + " = " + c.Value).ToList()
                };
                roles.Add(roleModel);
            }
        }
        public void OnPost()
        {
            RedirectToPage();
        }
    }
}
