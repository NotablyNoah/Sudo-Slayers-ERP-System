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
    public class InventorymgmtModel : PageModel
    {
        private readonly AppDbContent _db;
        public InventorymgmtModel(AppDbContent db) { _db = db; } 

        // Filters (persist via query string)
        [BindProperty(SupportsGet = true)]
        public string? CategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        // Input model for Add/Edit
        [BindProperty]
        public Inventory Input { get; set; } = new();

        // Data exposed to view
        public List<Inventory> Items { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        // Cards / metrics
        public int TotalItemsCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockCount { get; set; }
        public Inventory? TopItemByQty { get; set; }

        public async Task OnGetAsync()
        {
            // load categories
            Categories = await _db.Inventory
                .AsNoTracking()
                .Select(i => i.Category ?? "")
                .Where(c => c != "")
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // base query
            var q = _db.Inventory.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(CategoryFilter))
                q = q.Where(i => i.Category == CategoryFilter);

            if (!string.IsNullOrWhiteSpace(Search))
                q = q.Where(i => i.Name.Contains(Search) || i.SKU.Contains(Search));

            Items = await q.OrderBy(i => i.Name).ToListAsync();

            // metrics
            TotalItemsCount = await _db.Inventory.CountAsync();
            TotalInventoryValue = Math.Round(await _db.Inventory.SumAsync(i => (decimal?)i.Qty_On_Hand * i.Unit_Cost) ?? 0m, 2);
            LowStockCount = await _db.Inventory.CountAsync(i => i.Reorder_Level != null && i.Qty_On_Hand <= i.Reorder_Level);
            TopItemByQty = await _db.Inventory.OrderByDescending(i => i.Qty_On_Hand).FirstOrDefaultAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid) return await RefreshAndReturnPage();

            Input.Created_At = DateTime.UtcNow;
            Input.Updated_At = DateTime.UtcNow;

            _db.Inventory.Add(Input);
            await _db.SaveChangesAsync();

            return RedirectToPage(new { CategoryFilter = CategoryFilter, Search = Search });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var existing = await _db.Inventory.FindAsync(id);
            if (existing == null) return NotFound();

            if (!ModelState.IsValid) return await RefreshAndReturnPage();

            // update allowed fields
            existing.SKU = Input.SKU;
            existing.Name = Input.Name;
            existing.Category = Input.Category;
            existing.Description = Input.Description;
            existing.Qty_On_Hand = Input.Qty_On_Hand;
            existing.Unit_Cost = Input.Unit_Cost;
            existing.Unit_Price = Input.Unit_Price;
            existing.Reorder_Level = Input.Reorder_Level;
            existing.Location = Input.Location;
            existing.Last_Activity = Input.Last_Activity ?? DateTime.UtcNow;
            existing.Updated_At = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return RedirectToPage(new { CategoryFilter = CategoryFilter, Search = Search });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var existing = await _db.Inventory.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Inventory.Remove(existing);
            await _db.SaveChangesAsync();
            return RedirectToPage(new { CategoryFilter = CategoryFilter, Search = Search });
        }

        // helper to reload Items and metrics and return Page (used when ModelState invalid)
        private async Task<IActionResult> RefreshAndReturnPage()
        {
            await OnGetAsync();
            return Page();
        }
    }
}