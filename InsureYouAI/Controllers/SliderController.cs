using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers;

public class SliderController : Controller
{
    private readonly InsureContext _context;

    public SliderController(InsureContext context)
    {
        _context = context;
    }

    public IActionResult SliderList()
    {
        var values = _context.Sliders.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateSlider()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateSlider(Slider Slider)
    {
        _context.Sliders.Add(Slider);
        _context.SaveChanges();
        return RedirectToAction("SliderList");
    }
    [HttpGet]
    public IActionResult UpdateSlider(int Id)
    {
        var value = _context.Sliders.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateSlider(Slider Slider)
    {
        _context.Sliders.Update(Slider);
        _context.SaveChanges();
        return RedirectToAction("SliderList");
    }


    public IActionResult DeleteSlider(int Id)
    {
        var value = _context.Sliders.Find(Id);
        _context.Sliders.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("SliderList");
    }
}
