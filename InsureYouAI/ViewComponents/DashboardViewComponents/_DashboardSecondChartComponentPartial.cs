using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardSecondChartComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DashboardSecondChartComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Policies
                .GroupBy(x => x.PolicyType) // Artık Türe göre grupluyoruz
                .Select(g => new PolicyTypeViewModel
                {
                    PolicyTypeName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5) // En popüler 5 türü gösterelim (grafik boğulmasın)
                .ToList();

            return View(values);
        }
    }
}