using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers;
public class MessageController : Controller
{
    private readonly InsureContext _context;

    public MessageController(InsureContext context)
    {
        _context = context;
    }

    public IActionResult MessageList()
    {
        var values = _context.Messages.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateMessage()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateMessage(Message message)
    {
        message.IsRead = false;
        message.SendDate = DateTime.Now;
        _context.Messages.Add(message);
        _context.SaveChanges();
        return RedirectToAction("MessageList");
    }
    [HttpGet]
    public IActionResult UpdateMessage(int Id)
    {
        var value = _context.Messages.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateMessage(Message message)
    {
        _context.Messages.Update(message);
        _context.SaveChanges();
        return RedirectToAction("MessageList");
    }


    public IActionResult DeleteMessage(int Id)
    {
        var value = _context.Messages.Find(Id);
        _context.Messages.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("MessageList");
    }
}
