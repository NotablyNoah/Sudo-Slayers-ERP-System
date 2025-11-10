using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Pages
{
    public class VendormgmtModel : PageModel
    {
        private readonly AppDbContent _db;

        public VendormgmtModel(AppDbContent db)
        {
            _db = db;
        }

        // List shown in the table
        public List<VendorData> Vendors { get; set; } = new List<VendorData>();

        // Bound vendor for the form (add/edit)
        [BindProperty]
        public VendorData Vendor { get; set; } = new VendorData();

        // File upload bindings
        [BindProperty]
        public IFormFile? PhotoUpload { get; set; }

        [BindProperty]
        public IFormFile? DocumentUpload { get; set; }

        // Summary properties
        public int TotalVendors { get; set; }
        public int ActiveVendors { get; set; }
        public decimal AvgRating { get; set; }
        public int WithFiles { get; set; }

        // GET: load list, optionally load a vendor for editing when id is supplied
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Vendor")
            {
                TempData["ErrorMessage"] = "Access denied. Admins/Vendors only.";
                return RedirectToPage("/Index");
            }

            Vendors = await _db.VendorData
                .AsNoTracking()
                .OrderBy(v => v.Vendor_Name)
                .ToListAsync();

            ComputeSummaries();

            if (id.HasValue)
            {
                var existing = await _db.VendorData.FindAsync(id.Value);
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Vendor not found.";
                    return Page();
                }

                // Populate bound Vendor for editing
                Vendor = existing;
            }

            return Page();
        }

        // POST: save (create or update)
        public async Task<IActionResult> OnPostSaveAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Vendor")
            {
                TempData["ErrorMessage"] = "Access denied. Admins/Vendors only.";
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                // reload vendors & summaries for redisplay
                Vendors = await _db.VendorData.AsNoTracking().OrderBy(v => v.Vendor_Name).ToListAsync();
                ComputeSummaries();
                return Page();
            }

            // Process uploads into Vendor fields (if provided)
            if (PhotoUpload != null && PhotoUpload.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoUpload.CopyToAsync(ms);
                Vendor.PhotoFileName = PhotoUpload.FileName;
            }

            if (DocumentUpload != null && DocumentUpload.Length > 0)
            {
                using var ms = new MemoryStream();
                await DocumentUpload.CopyToAsync(ms);
                Vendor.DocumentFileName = DocumentUpload.FileName;
            }

            if (Vendor.Vendor_ID == 0)
            {
                // Create
                Vendor.Created_At = DateTime.UtcNow;
                Vendor.Updated_At = DateTime.UtcNow;
                _db.VendorData.Add(Vendor);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Vendor added.";
            }
            else
            {
                // Update
                var existing = await _db.VendorData.FindAsync(Vendor.Vendor_ID);
                if (existing == null)
                {
                    TempData["Error"] = "Vendor not found.";
                    return RedirectToPage();
                }

                // Map updatable fields
                existing.Vendor_Name = Vendor.Vendor_Name;
                existing.Contact_Person = Vendor.Contact_Person;
                existing.Email = Vendor.Email;
                existing.Vendor_Type = Vendor.Vendor_Type;
                existing.Status = Vendor.Status;
                existing.Rating = Vendor.Rating;
                existing.Updated_At = DateTime.UtcNow;

                _db.VendorData.Update(existing);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Vendor updated.";
            }

            // Redirect to GET to clear model binding and avoid reposts
            return RedirectToPage();
        }

        // POST: delete vendor
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Vendor")
            {
                TempData["ErrorMessage"] = "Access denied. Admins/Vendors only.";
                return RedirectToPage("/Index");
            }

            var existing = await _db.VendorData.FindAsync(id);
            if (existing != null)
            {
                _db.VendorData.Remove(existing);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Vendor deleted.";
            }
            else
            {
                TempData["Error"] = "Vendor not found.";
            }
            return RedirectToPage();
        }

        // Helper to compute summary card values
        private void ComputeSummaries()
        {
            TotalVendors = Vendors?.Count ?? 0;
            ActiveVendors = Vendors?.Count(v => string.Equals(v.Status, "active", StringComparison.OrdinalIgnoreCase)) ?? 0;
            AvgRating = Vendors != null && Vendors.Count > 0 ? Math.Round((decimal)(Vendors.Average(v => v.Rating ?? 0)), 2) : 0m;
        }
    }
}