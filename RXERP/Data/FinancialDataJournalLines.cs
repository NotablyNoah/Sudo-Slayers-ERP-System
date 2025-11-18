using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RXERP.Data
{
    [Table("financial_journal_lines")]
    [Index(nameof(Journal_ID), Name = "idx_fin_journal_lines_journal")]
    public class FinancialDataJournalLine
    {
        [Key]
        [Column("line_id")]
        public int Line_ID { get; set; }

        [Required]
        [Column("journal_id")]
        public int Journal_ID { get; set; }

        [MaxLength(100)]
        [Column("account_code")]
        public string? Account_Code { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("description")]
        public string Description { get; set; } = null!;

        // debit and credit recorded as DECIMAL(12,2)
        [Required]
        [Column("debit", TypeName = "decimal(12,2)")]
        public decimal Debit { get; set; } = 0.00m;

        [Required]
        [Column("credit", TypeName = "decimal(12,2)")]
        public decimal Credit { get; set; } = 0.00m;

        [Required]
        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
}