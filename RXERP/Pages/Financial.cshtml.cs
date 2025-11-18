using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Pages
{
    public class FinancialModel : PageModel
    {
        private readonly AppDbContent _db;

        public FinancialModel(AppDbContent db)
        {
            _db = db;
        }

        // Invoice lines shown in the table (loaded from DB)
        public List<FinancialDataInvoiceLine> Lines { get; set; } = new List<FinancialDataInvoiceLine>();

        // Bound model for Add / Edit form (server-side)
        [BindProperty]
        public FinancialLineInput LineInput { get; set; } = new FinancialLineInput();

        // Optional filter (supports GET)
        [BindProperty(SupportsGet = true)]
        public int? FilterInvoiceId { get; set; }

        // Summary values computed from Lines
        public int TotalLines => Lines?.Count ?? 0;
        public decimal SubtotalTotal => Math.Round(Lines?.Sum(l => l.Line_Subtotal) ?? 0m, 2);
        public decimal TaxTotal => Math.Round(Lines?.Sum(l => l.Tax_Amount) ?? 0m, 2);
        public decimal GrandTotal => Math.Round(Lines?.Sum(l => l.Line_Total) ?? 0m, 2);

        // GET: Load invoice lines optionally filtered by invoice id
        public async Task<IActionResult> OnGetAsync(int? invoiceId)
        {
            // Optional: keep existing role check if you want to restrict access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Finance" && userRole != "Vendor")
            {
                TempData["ErrorMessage"] = "Access denied. Admins/Finance/Vendors only.";
                return RedirectToPage("/Index");
            }

            if (invoiceId.HasValue)
                FilterInvoiceId = invoiceId.Value;

            await LoadLinesAsync();
            return Page();
        }

        // POST: Add a new invoice line
        public async Task<IActionResult> OnPostAddLineAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLinesAsync();
                return Page();
            }

            var entity = new FinancialDataInvoiceLine
            {
                Invoice_ID = LineInput.Invoice_ID,
                Line_Number = LineInput.Line_Number,
                Item_Code = LineInput.Item_Code,
                Description = LineInput.Description!,
                Quantity = LineInput.Quantity,
                Unit_Price = LineInput.Unit_Price,
                Tax_Amount = LineInput.Tax_Amount,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow,
                Deleted_At = null
            };

            // compute subtotal and total server-side
            entity.Line_Subtotal = Math.Round(entity.Quantity * entity.Unit_Price, 2);
            entity.Line_Total = Math.Round(entity.Line_Subtotal + entity.Tax_Amount, 2);

            _db.Set<FinancialDataInvoiceLine>().Add(entity);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Invoice line added.";
            return RedirectToPage(new { invoiceId = entity.Invoice_ID });
        }

        // POST: Update existing line (LineInput must contain Line_ID)
        public async Task<IActionResult> OnPostUpdateLineAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLinesAsync();
                return Page();
            }

            if (LineInput.Line_ID == 0)
            {
                TempData["Error"] = "Line ID missing for update.";
                return RedirectToPage();
            }

            var existing = await _db.Set<FinancialDataInvoiceLine>().FindAsync(LineInput.Line_ID);
            if (existing == null)
            {
                TempData["Error"] = "Invoice line not found.";
                return RedirectToPage();
            }

            existing.Invoice_ID = LineInput.Invoice_ID;
            existing.Line_Number = LineInput.Line_Number;
            existing.Item_Code = LineInput.Item_Code;
            existing.Description = LineInput.Description!;
            existing.Quantity = LineInput.Quantity;
            existing.Unit_Price = LineInput.Unit_Price;
            existing.Tax_Amount = LineInput.Tax_Amount;
            existing.Line_Subtotal = Math.Round(existing.Quantity * existing.Unit_Price, 2);
            existing.Line_Total = Math.Round(existing.Line_Subtotal + existing.Tax_Amount, 2);
            existing.Updated_At = DateTime.UtcNow;

            _db.Set<FinancialDataInvoiceLine>().Update(existing);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Invoice line updated.";
            return RedirectToPage(new { invoiceId = existing.Invoice_ID });
        }

        // POST: Delete a line by id
        public async Task<IActionResult> OnPostDeleteLineAsync(int id)
        {
            var existing = await _db.Set<FinancialDataInvoiceLine>().FindAsync(id);
            if (existing != null)
            {
                // hard-delete; if you prefer soft-delete set Deleted_At
                _db.Set<FinancialDataInvoiceLine>().Remove(existing);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Invoice line deleted.";
                return RedirectToPage(new { invoiceId = existing.Invoice_ID });
            }

            TempData["Error"] = "Invoice line not found.";
            return RedirectToPage();
        }

        // Helper: load lines from DB into the Lines list
        private async Task LoadLinesAsync()
        {
            var q = _db.Set<FinancialDataInvoiceLine>().AsNoTracking().AsQueryable();
            if (FilterInvoiceId.HasValue)
                q = q.Where(x => x.Invoice_ID == FilterInvoiceId.Value);

            Lines = await q.OrderBy(x => x.Line_ID).ToListAsync();
            Lines = Lines ?? new List<FinancialDataInvoiceLine>();
        }

        // Input DTO for binding form posts
        public class FinancialLineInput
        {
            [Display(Name = "Line ID")]
            public int Line_ID { get; set; } = 0;

            [Required]
            [Display(Name = "Invoice ID")]
            public int Invoice_ID { get; set; }

            [Required]
            [Display(Name = "Line Number")]
            public int Line_Number { get; set; }

            [StringLength(100)]
            [Display(Name = "Item Code")]
            public string? Item_Code { get; set; }

            [Required]
            [StringLength(500)]
            [Display(Name = "Description")]
            public string? Description { get; set; }

            [Required]
            [Range(0.01, 1000000000)]
            [Display(Name = "Quantity")]
            public decimal Quantity { get; set; } = 1.00m;

            [Required]
            [Range(0, 1000000000)]
            [Display(Name = "Unit Price")]
            public decimal Unit_Price { get; set; } = 0.0000m;

            [Required]
            [Range(0, 1000000000)]
            [Display(Name = "Tax Amount")]
            public decimal Tax_Amount { get; set; } = 0.00m;
        }
    }
}