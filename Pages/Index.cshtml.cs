using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ASP.NET_Razor_Final.Pages;


public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _myBlogContext;
    public IndexModel(ILogger<IndexModel> logger, AppDbContext myBlogContext)
    {
        _logger = logger;
        _myBlogContext = myBlogContext;
    }
    public IList<Article> posts { set; get; }
    public void OnGet(int? id)
    {
        posts = (from article in _myBlogContext.articles
                   orderby article.Created descending
                   select article).ToList();
        if (id != null)
        {
            var kq = posts.Where(a => a.Id == id).FirstOrDefault();
            ViewData["q"] = kq;
        }
        ViewData["post"] = posts.ToList();
    }
}
//dotnet aspnet-codegenerator razorpage -m ASP.NET_Razor_Final.models.Article -dc ASP.NET_Razor_Final.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
//dotnet aspnet-codegenerator razorpage -m ASP.NET_Razor_Final.Models.Article -dc ASP.NET_Razor_Final.Pages.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
