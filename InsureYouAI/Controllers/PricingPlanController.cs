using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers;

public class PricingPlanController : Controller
{
    private readonly InsureContext _context;

    public PricingPlanController(InsureContext context)
    {
        _context = context;
    }

    public IActionResult PricingPlanList()
    {
        var values = _context.PricingPlans.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreatePricingPlan()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreatePricingPlan(PricingPlan pricingPlan)
    {
        _context.PricingPlans.Add(pricingPlan);
        _context.SaveChanges();
        return RedirectToAction("PricingPlanList");
    }
    [HttpGet]
    public IActionResult UpdatePricingPlan(int Id)
    {
        var value = _context.PricingPlans.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdatePricingPlan(PricingPlan pricingPlan)
    {
        _context.PricingPlans.Update(pricingPlan);
        _context.SaveChanges();
        return RedirectToAction("PricingPlanList");
    }


    public IActionResult DeletePricingPlan(int Id)
    {
        var value = _context.PricingPlans.Find(Id);
        _context.PricingPlans.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("PricingPlanList");
    }
}
