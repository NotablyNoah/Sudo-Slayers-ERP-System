using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RXERP.Data
{
    [Table("Finance_Invoice_Lines")]
    public class FinancialDataInvoiceLine
    {
        [Key]
        [Column("line_id")]
        public int Line_ID { get; set; }

        [Required]
        [Column("invoice_id")]
        public int Invoice_ID { get; set; }

        [Required]
        [Column("line_number")]
        public int Line_Number { get; set; }

        [MaxLength(100)]
        [Column("item_code")]
        public string? Item_Code { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("description")]
        public string Description { get; set; } = null!;

        // quantity DECIMAL(12,2)
        [Required]
        [Column("quantity", TypeName = "decimal(12,2)")]
        public decimal Quantity { get; set; } = 1.00m;

        // unit_price DECIMAL(12,4)
        [Required]
        [Column("unit_price", TypeName = "decimal(12,4)")]
        public decimal Unit_Price { get; set; } = 0.0000m;

        // line_subtotal DECIMAL(12,2) (should be calculated as Quantity * Unit_Price)
        [Required]
        [Column("line_subtotal", TypeName = "decimal(12,2)")]
        public decimal Line_Subtotal { get; set; }

        [Required]
        [Column("tax_amount", TypeName = "decimal(12,2)")]
        public decimal Tax_Amount { get; set; } = 0.00m;

        [Required]
        [Column("line_total", TypeName = "decimal(12,2)")]
        public decimal Line_Total { get; set; }

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