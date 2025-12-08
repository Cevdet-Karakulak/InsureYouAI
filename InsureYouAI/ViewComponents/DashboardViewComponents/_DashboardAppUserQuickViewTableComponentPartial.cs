using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardAppUserQuickViewTableComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DashboardAppUserQuickViewTableComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Users
                .Select(u => new AppUserQuickViewVM
                {
                    UserId = u.Id,
                    FullName = u.Name + " " + u.Surname,
                    ImageUrl = u.ImageUrl,

                    PolicyCount = _context.Policies.Count(p => p.AppUserId == u.Id),
                    TotalBalance = _context.Policies
                        .Where(p => p.AppUserId == u.Id)
                        .Sum(p => (decimal?)p.PremiumAmount) ?? 0
                })
                .OrderByDescending(x => x.TotalBalance) // en çok kazandıran üstte
                .AsNoTracking()
                .ToList();

            return View(values);
        }
    }
}
