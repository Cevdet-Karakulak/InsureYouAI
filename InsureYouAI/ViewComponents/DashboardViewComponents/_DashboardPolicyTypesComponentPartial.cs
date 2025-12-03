using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardPolicyTypesComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DashboardPolicyTypesComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
           var result =  _context.Policies.GroupBy(p => p.PolicyType)
                        .Select(g => new PolicyGroupViewModel
                        {
                            PolicyType = g.Key,
                            Count = g.Count()
                        })
                        .ToList();

            return View(result);
        }
    }
}
