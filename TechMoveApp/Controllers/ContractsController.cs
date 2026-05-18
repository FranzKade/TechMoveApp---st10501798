using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveApp.Data;
using TechMoveApp.Models;

namespace TechMoveApp.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContractsController(TechMoveDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. GET: Contracts with Advanced LINQ Filtering (Requirement 2)
        public async Task<IActionResult> Index(ContractStatus? status, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            // Filter by Status using LINQ
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // Filter by Date Range using LINQ
            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            // Retain values in ViewBag for the UI filter inputs
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedEndDate = endDate?.ToString("yyyy-MM-dd");

            return View(await query.ToListAsync());
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "Name");
            return View();
        }

        // 2. POST: Contracts with Strict PDF Validation (Requirement 1 & 4)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (signedAgreement != null && signedAgreement.Length > 0)
            {
                // Strict File Type Validation Check (.pdf only)
                var fileExtension = Path.GetExtension(signedAgreement.FileName).ToLowerInvariant();
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("SignedAgreementFilePath", "Security Validation: Document type restricted to PDF format.");

                    // Reload dropdown list on error
                    ViewBag.Clients = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "Name");
                    return View(contract);
                }

                // Create unique local file vault destination paths
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "DocumentVault");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(signedAgreement.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await signedAgreement.CopyToAsync(fileStream);
                }

                contract.SignedAgreementFilePath = Path.Combine("DocumentVault", uniqueFileName);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "Name");
                return View(contract);
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. GET: Stream/Download Agreement Document
        public IActionResult DownloadAgreement(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();

            string fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/pdf", Path.GetFileName(fullPath));
        }
    }
}