using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Razor_Final.models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Razor_Final.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ASP.NET_Razor_Final.models.AppDbContext _context;

        public IndexModel(ASP.NET_Razor_Final.models.AppDbContext context)
        {
            _context = context;
        }
        public IList<Article> Article { get; set; } = default!; // default! là cú pháp trong C# để gán giá trị mặc định cho biến, 
                                                                // đồng thời dùng dấu ! (null-forgiving operator) để báo với trình biên dịch rằng biến này sẽ không null.
                                                                // Ở đây, Article được khởi tạo với giá trị mặc định (null cho reference type), 
                                                                // nhưng dấu ! giúp tránh cảnh báo nullable.
        public const int ITEM_PER_PAGE = 15;

        [BindProperty(SupportsGet = true, Name = "trang")]
        public int currentPage { set; get; }

        public int countPages { get; set; }

        public string SearchString { set; get; }

        public async Task OnGetAsync(string SearchString)
        {
            this.SearchString = SearchString;
            int totalArticle = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEM_PER_PAGE);
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;

            var query = (from article in _context.articles
                         orderby article.Created descending
                         select article)
                        .Skip((currentPage - 1) * 10)
                        .Take(ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(SearchString))
            {
                Article = await query.Where(a => a.Title.Contains(SearchString)).ToListAsync();
            }
            else
                Article = await query.ToListAsync();
        }
    }
}
