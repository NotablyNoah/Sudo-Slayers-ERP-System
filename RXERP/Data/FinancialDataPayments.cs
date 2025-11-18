using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RXERP.Data
{
    [Table("finance_payments")]
    [Index(nameof(Vendor_ID), Name = "idx_fin_payments_vendor")]
    [Index(nameof(Reference_Number), Name = "idx_fin_payments_ref")]
    public class FinancialDataPayment
    {
        [Key]
        [Column("payment_id")]
        public int Payment_ID { get; set; }

        [Required]
        [Column("vendor_id")]
        public int Vendor_ID { get; set; }

        [Required]
        [Column("payment_date", TypeName = "date")]
        public DateTime Payment_Date { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("payment_method")]
        public string Payment_Method { get; set; } = null!;

        // If you actually want the column name to start with an underscore use Column("_reference_number")
        [MaxLength(100)]
        [Column("reference_number")]
        public string? Reference_Number { get; set; }

        [Required]
        [Column("amount_total", TypeName = "decimal(12,2)")]
        public decimal Amount_Total { get; set; }

        [Column("notes", TypeName = "text")]
        public string? Notes { get; set; }

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