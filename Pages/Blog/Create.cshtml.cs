using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Razor_Final.Pages_Blog
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ASP.NET_Razor_Final.models.AppDbContext _context;

        public CreateModel(ASP.NET_Razor_Final.models.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        [TempData]
        public string StatusMessage { set; get; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.articles.Add(Article);
            await _context.SaveChangesAsync();
            StatusMessage = $@"Đã tạo bài viết ""{Article.Title}"" ";
            return RedirectToPage("./Index");
        }
    }
}
