using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RXERP.Data
{
    [Table("Finance_Journals")]
    [Index(nameof(Reference_ID), Name = "idx_fin_journals_reference")]
    public class FinancialDataJournal
    {
        [Key]
        [Column("journal_id")]
        public int Journal_ID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("journal_type")]
        public string Journal_Type { get; set; } = null!;

        [Column("reference_id")]
        public int? Reference_ID { get; set; }

        [MaxLength(50)]
        [Column("reference_type")]
        public string? Reference_Type { get; set; }

        [Required]
        [Column("journal_date", TypeName = "date")]
        public DateTime Journal_Date { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("description")]
        public string Description { get; set; } = null!;

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