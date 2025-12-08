using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RXERP.Data
{
    [Table("inventory")]
    public class Inventory
    {
        [Key]
        [Column("item_id")]
        public int Item_ID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("sku")]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("category")]
        public string? Category { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Column("qty_on_hand", TypeName = "decimal(12,2)")]
        public decimal Qty_On_Hand { get; set; }

        [Column("unit_cost", TypeName = "decimal(12,4)")]
        public decimal Unit_Cost { get; set; }

        [Column("unit_price", TypeName = "decimal(12,2)")]
        public decimal Unit_Price { get; set; }

        [Column("reorder_level", TypeName = "decimal(12,2)")]
        public decimal? Reorder_Level { get; set; }

        [MaxLength(100)]
        [Column("location")]
        public string? Location { get; set; }

        [Column("last_activity")]
        public DateTime? Last_Activity { get; set; }

        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;
    }
}