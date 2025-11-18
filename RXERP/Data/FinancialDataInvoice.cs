using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RXERP.Data
{
    [Table("Finance_Invoices")]
    [Index(nameof(Invoice_Number), IsUnique = true)]
    public class FinancialDataInvoice
    {
        [Key]
        [Column("invoice_id")]
        public int Invoice_ID { get; set; }

        [Required]
        [Column("vendor_id")]
        public int Vendor_ID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("invoice_number")]
        public string Invoice_Number { get; set; } = null!;

        [Required]
        [Column("invoice_date")]
        public DateTime Invoice_Date { get; set; }

        [Column("due_date")]
        public DateTime? Due_Date { get; set; }

        // DECIMAL(12,2)
        [Required]
        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column("tax_amount", TypeName = "decimal(12,2)")]
        public decimal Tax_Amount { get; set; }

        [Required]
        [Column("total_amount", TypeName = "decimal(12,2)")]
        public decimal Total_Amount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "open";

        [Required]
        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }
    }
}