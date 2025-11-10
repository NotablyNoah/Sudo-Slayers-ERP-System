// Data/Vendor.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RXERP.Data;

[Table("vendors")]
public class VendorData
{
    [Key]
    public int Vendor_ID { get; set; }

    [Column]
    [Required]
    public string Vendor_Name { get; set; } = null!;

    [Column]
    public string? Contact_Person { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Column]
    public string? Vendor_Type { get; set; }

    [Required]
    [Column]
    public string Status { get; set; } = "active";

    [Column]
    public decimal? Rating { get; set; }

    [Column]
    public DateTime Created_At { get; set; } = DateTime.UtcNow;
    [Column]
    public DateTime Updated_At { get; set; } = DateTime.UtcNow;

    [Column]
    public string? PhotoFileName { get; set; }
    [Column]
    public string? DocumentFileName { get; set; }
}