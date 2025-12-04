using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Pages
{
    public class CRMModel : PageModel
    {
        private readonly AppDbContent _db;

        public CRMModel(AppDbContent db)
        {
            _db = db;
        }

        // exposed to the Razor page
        public List<CRMData> Customers { get; set; } = new();

        [BindProperty]
        public string ReviewContent { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            // Role-based access (Customer and Admin allowed)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Customer" && userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage("/Index");
            }

            Customers = await _db.CRMData
                .AsNoTracking()
                .OrderByDescending(c => c.Created_At)
                .ToListAsync();

            return Page();
        }

        // Handler for the review form
        public async Task<IActionResult> OnPostReviewAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Customer")
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage("/Index");
            }

            var entry = new CRMData
            {
                Name = "Anonymous",
                Email = "anonymous@example.com",
                Review = ReviewContent,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow
            };

            _db.CRMData.Add(entry);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        // Handler to save edits from the modal
        public async Task<IActionResult> OnPostEditCustomerAsync(int Customer_ID, string Name, string Email, string Review)
        {
            var customer = await _db.CRMData.FindAsync(Customer_ID);
            if (customer == null)
                return NotFound();

            customer.Name = Name;
            customer.Email = Email;
            customer.Review = Review;
            customer.Updated_At = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        // Handler to delete a customer entry
        public async Task<IActionResult> OnPostDeleteCustomerAsync(int Customer_ID)
        {
            var customer = await _db.CRMData.FindAsync(Customer_ID);
            if (customer == null)
                return NotFound();

            _db.CRMData.Remove(customer);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}