using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace MyApp.Admin.Pages.Roles;

public class RolePageModel : PageModel
{
    protected readonly RoleManager<IdentityRole> _roleManager;
    protected readonly AppDbContext _dbContext;
    [TempData]
    public string StatusMessage { set; get; }
    public RolePageModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
    {
        _roleManager = roleManager;
        _dbContext = dbContext;
    }
}