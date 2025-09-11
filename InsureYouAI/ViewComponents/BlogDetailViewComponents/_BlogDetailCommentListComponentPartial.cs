using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailCommentListComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;
        public _BlogDetailCommentListComponentPartial(InsureContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(int id)
        {
            var values = _context.Comments.Where(x => x.ArticleId == id && x.CommentStatus == "Yorum Onaylandı").Include(y => y.AppUser).ToList();
            return View(values);
        }
    }
}
