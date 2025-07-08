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
    [Authorize(Roles = "Admin,Admin+Vip")]
    public class DeleteModel : PageModel
    {
        private readonly ASP.NET_Razor_Final.models.AppDbContext _context;

        public DeleteModel(ASP.NET_Razor_Final.models.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return Content("Không tìm thấy bài viết");
            }

            var article = await _context.articles.FindAsync(id);
            if (article != null)
            {
                // Nếu một thực thể với khóa chính đã cho đang được theo dõi bởi ngữ cảnh,
                // EF sẽ trả về thực thể đó thay vì truy vấn cơ sở dữ liệu.
                // Gán Article = article để đảm bảo thực thể đúng được xóa.
                // "Theo dõi" ở đây nghĩa là Entity Framework (EF) đang quản lý thực thể này trong bộ nhớ (change tracker).
                // Khi bạn truy vấn hoặc tìm một thực thể, EF sẽ theo dõi các thay đổi của nó để khi gọi SaveChanges(), các thay đổi đó sẽ được lưu vào cơ sở dữ liệu.
                Article = article;
                _context.articles.Remove(Article);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
