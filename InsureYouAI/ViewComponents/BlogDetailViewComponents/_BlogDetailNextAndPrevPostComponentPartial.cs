using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailNextAndPrevPostComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogDetailNextAndPrevPostComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            var current = _context.Articles.FirstOrDefault(x => x.ArticleId == id);
            if (current == null)
            {
                return View(new NextPrevArticleViewModel());
            }

            var prev = _context.Articles
                .Where(x => x.ArticleId < id)
                .OrderByDescending(x => x.ArticleId)
                .FirstOrDefault();

            var next = _context.Articles
                .Where(x => x.ArticleId > id)
                .OrderBy(x => x.ArticleId)
                .FirstOrDefault();

            string Truncate(string s, int len)
            {
                if (string.IsNullOrEmpty(s)) return s;
                return s.Length <= len ? s : s.Substring(0, len) + "...";
            }

            var vm = new NextPrevArticleViewModel
            {
                PrevId = prev?.ArticleId,
                PrevTitle = prev != null ? Truncate(prev.Title, 40) : null,
                NextId = next?.ArticleId,
                NextTitle = next != null ? Truncate(next.Title, 40) : null
            };

            return View(vm);
        }
    }
}
