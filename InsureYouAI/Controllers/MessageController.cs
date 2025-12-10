using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Services; // <-- AI Service
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class MessageController : Controller
    {
        private readonly InsureContext _context;
        private readonly AiMessageClassifierService _aiService;

        public MessageController(InsureContext context, AiMessageClassifierService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public IActionResult MessageList()
        {
            ViewBag.ControllerName = "Gelen Mesajlar";
            ViewBag.PageName = "İletişim Panelinden Gönderilen Mesajlar";
            var values = _context.Messages.OrderByDescending(x => x.SendDate).ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {
            message.IsRead = false;
            message.SendDate = DateTime.Now;

            // 🧠 AI Analiz
            message.AiCategory = await _aiService.DetectCategoryAsync(message.MessagetDetail);
            message.Priority = await _aiService.DetectPriorityAsync(message.MessagetDetail);

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("MessageList");
        }


        [HttpGet]
        public IActionResult UpdateMessage(int id)
        {
            var value = _context.Messages.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }

        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var message = _context.Messages.FirstOrDefault(x => x.MessageId == id);

            if (message == null)
                return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return View(message);
        }
    }
}
