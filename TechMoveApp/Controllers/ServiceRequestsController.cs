using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveApp.Data;
using TechMoveApp.Models;
using TechMoveApp.Services;

namespace TechMoveApp.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly ExchangeRateService _exchangeRateService;

        public ServiceRequestsController(TechMoveDbContext context, ExchangeRateService exchangeRateService)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<IActionResult> Index() => View(await _context.ServiceRequests.Include(s => s.Contract).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);

            // Strict Workflow Logic Rule (Requirement 2)
            if (contract == null || contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                TempData["ErrorMessage"] = "Validation Alert: New requests cannot be mapped onto an Expired or OnHold contract container.";
                return RedirectToAction("Index", "Contracts");
            }

            decimal currentRate = await _exchangeRateService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = currentRate;
            ViewBag.ContractId = contractId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                decimal currentRate = await _exchangeRateService.GetUsdToZarRateAsync();
                ViewBag.ExchangeRate = currentRate;
                ViewBag.ContractId = request.ContractId;
                return View(request);
            }

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}