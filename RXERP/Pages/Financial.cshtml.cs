using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // Loaded tables
        public List<FinancialDataInvoice> Invoices { get; set; } = new();
        public List<FinancialDataInvoiceLine> InvoiceLines { get; set; } = new();
        public List<FinancialDataJournal> Journals { get; set; } = new();
        public List<FinancialDataJournalLine> JournalLines { get; set; } = new();
        public List<FinancialDataPayment> Payments { get; set; } = new();

        // Summary values for cards
        public int InvoiceCount { get; set; }
        public int PaymentCount { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Outstanding { get; set; }

        // Vendors owing (basic DTO)
        public List<VendorOwingDto> VendorsOwing { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load sequentially (recommended for simplicity and reliability)
            await LoadInvoicesAsync();
            await LoadInvoiceLinesAsync();
            await LoadJournalsAsync();
            await LoadJournalLinesAsync();
            await LoadPaymentsAsync();

            // Compute simple summaries for the cards
            InvoiceCount = Invoices.Count;
            PaymentCount = Payments.Count;
            TotalInvoiced = Math.Round(Invoices.Sum(i => i.Total_Amount), 2);
            TotalRevenue = Math.Round(Payments.Sum(p => p.Amount_Total), 2);
            Outstanding = Math.Round(TotalInvoiced - TotalRevenue, 2);

            // Compute vendors owing (in-memory from loaded lists)
            var invoiceSums = Invoices
                .GroupBy(i => i.Vendor_ID)
                .Select(g => new { Vendor_ID = g.Key, Invoiced = g.Sum(x => x.Total_Amount) });

            var paymentSums = Payments
                .GroupBy(p => p.Vendor_ID)
                .Select(g => new { Vendor_ID = g.Key, Paid = g.Sum(x => x.Amount_Total) });

            var join = from inv in invoiceSums
                       join pay in paymentSums on inv.Vendor_ID equals pay.Vendor_ID into payg
                       from pay in payg.DefaultIfEmpty()
                       select new
                       {
                           Vendor_ID = inv.Vendor_ID,
                           Invoiced = inv.Invoiced,
                           Paid = pay?.Paid ?? 0m,
                           Outstanding = inv.Invoiced - (pay?.Paid ?? 0m)
                       };

            var oweList = join.Where(x => x.Outstanding > 0).OrderByDescending(x => x.Outstanding).ToList();

            VendorsOwing = oweList.Select(x => new VendorOwingDto
            {
                Vendor_ID = x.Vendor_ID,
                Vendor_Name = "(vendor)",
                InvoicedTotal = Math.Round(x.Invoiced, 2),
                PaidTotal = Math.Round(x.Paid, 2),
                Outstanding = Math.Round(x.Outstanding, 2)
            }).ToList();

            // Resolve vendor names via VendorData table if present
            var vendorIds = VendorsOwing.Select(v => v.Vendor_ID).Distinct().ToList();
            if (vendorIds.Any())
            {
                var vendors = await _db.Set<VendorData>()
                    .AsNoTracking()
                    .Where(v => vendorIds.Contains(v.Vendor_ID))
                    .ToDictionaryAsync(v => v.Vendor_ID, v => v.Vendor_Name);

                foreach (var vo in VendorsOwing)
                    vo.Vendor_Name = vendors.TryGetValue(vo.Vendor_ID, out var n) ? n : "(vendor)";
            }
        }

        private async Task LoadInvoicesAsync()
        {
            Invoices = await _db.Set<FinancialDataInvoice>().AsNoTracking()
                .OrderByDescending(i => i.Invoice_Date).ToListAsync();
        }

        private async Task LoadInvoiceLinesAsync()
        {
            InvoiceLines = await _db.Set<FinancialDataInvoiceLine>().AsNoTracking()
                .OrderBy(l => l.Line_Number).ToListAsync();
        }

        private async Task LoadJournalsAsync()
        {
            Journals = await _db.Set<FinancialDataJournal>().AsNoTracking()
                .OrderByDescending(j => j.Journal_Date).ToListAsync();
        }

        private async Task LoadJournalLinesAsync()
        {
            JournalLines = await _db.Set<FinancialDataJournalLine>().AsNoTracking()
                .OrderBy(jl => jl.Line_ID).ToListAsync();
        }

        private async Task LoadPaymentsAsync()
        {
            Payments = await _db.Set<FinancialDataPayment>().AsNoTracking()
                .OrderByDescending(p => p.Payment_Date).ToListAsync();
        }

        // DTO
        public class VendorOwingDto
        {
            public int Vendor_ID { get; set; }
            public string Vendor_Name { get; set; } = string.Empty;
            public decimal InvoicedTotal { get; set; }
            public decimal PaidTotal { get; set; }
            public decimal Outstanding { get; set; }
        }
    }
}