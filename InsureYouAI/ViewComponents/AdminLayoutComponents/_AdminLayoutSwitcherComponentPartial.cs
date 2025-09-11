using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponenets.AdminLayoutComponents;

public class _AdminLayoutSwitcherComponentPartial:ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
