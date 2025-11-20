using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RXERP.Data
{
    [Table("finance_taxrates")]
    [Index(nameof(Code), Name = "idx_taxrates_code")]
    [Index(nameof(Region), Name = "idx_taxrates_region")]
    public class FinancialDataTaxRate
    {
        [Key]
        [Column("tax_rate_id")]
        public int TaxRateID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code")]
        public string Code { get; set; } = null!;

        [MaxLength(100)]
        [Column("region")]
        public string? Region { get; set; }

        // Rate stored with 4 decimal places (e.g. 0.0750 for 7.50%)
        [Required]
        [Column("rate", TypeName = "decimal(6,4)")]
        public decimal Rate { get; set; }

        [Column("effective_date", TypeName = "date")]
        public DateTime? EffectiveDate { get; set; }

        [Column("expiration_date", TypeName = "date")]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}