using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Razor_Final.Pages_Blog
{
    [Authorize] //Nam sinh tu 1997-2012 thi duoc truy cap
    public class DetailsModel : PageModel
    {
        private readonly ASP.NET_Razor_Final.models.AppDbContext _context;

        public DetailsModel(ASP.NET_Razor_Final.models.AppDbContext context)
        {
            _context = context;
        }

        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);

            if (article is not null)
            {
                Article = article;

                return Page();
            }

            return NotFound();
        }
    }
}
