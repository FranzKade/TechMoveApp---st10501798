using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveApp.Data;
using TechMoveApp.Models;

namespace TechMoveApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly TechMoveDbContext _context;
        public ClientsController(TechMoveDbContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Clients.ToListAsync());

        [HttpGet] public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid) return View(client);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}