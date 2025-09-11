using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers;
public class ContactController : Controller
{
    private readonly InsureContext _context;

    public ContactController(InsureContext context)
    {
        _context = context;
    }

    public IActionResult ContactList()
    {
        var values = _context.Contacts.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateContact()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateContact(Contact Contact)
    {
        _context.Contacts.Add(Contact);
        _context.SaveChanges();
        return RedirectToAction("ContactList");
    }
    [HttpGet]
    public IActionResult UpdateContact(int Id)
    {
        var value = _context.Contacts.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateContact(Contact Contact)
    {
        _context.Contacts.Update(Contact);
        _context.SaveChanges();
        return RedirectToAction("ContactList");
    }


    public IActionResult DeleteContact(int Id)
    {
        var value = _context.Contacts.Find(Id);
        _context.Contacts.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("ContactList");
    }
}
