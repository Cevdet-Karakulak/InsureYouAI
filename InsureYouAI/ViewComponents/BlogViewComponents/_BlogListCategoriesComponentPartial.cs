using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.BlogViewComponents
{
    public class _BlogListCategoriesComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;
        public _BlogListCategoriesComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            // Kategorileri Articles ile birlikte getir
            var values = _context.Categories
     .Include(c => c.Articles) // Articles koleksiyonunu yükle
     .ToList();

            return View(values);
        }
    }
}
