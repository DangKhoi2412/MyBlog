using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Razor_Final.models;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Razor_Final.Pages_Blog
{
    [Authorize(Roles ="Admin,Admin+Vip")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(AppDbContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        [TempData]
        public string StatusMessage { set; get; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return Content("Không tìm thấy bài viết");
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return Content("Không tìm thấy bài viết");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                //kiem tra quyen
                var IsCanUpdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                if (IsCanUpdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Khong duoc quyen cap nhat");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
                {
                    return Content("Không tìm thấy bài viết");
                }
                else
                {
                    throw;
                }
            }
            StatusMessage = $@"Đã chỉnh sửa bài viết ""{Article.Title}"" ";
            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.articles.Any(e => e.Id == id);
        }
    }
}
