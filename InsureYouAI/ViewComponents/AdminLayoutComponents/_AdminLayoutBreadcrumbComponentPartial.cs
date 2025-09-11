using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponenets.AdminLayoutComponents;

public class _AdminLayoutBreadcrumbComponentPartial:ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
