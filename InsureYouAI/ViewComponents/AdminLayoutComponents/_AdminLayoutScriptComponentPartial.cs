using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponenets.AdminLayoutComponents;

public class _AdminLayoutScriptComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
