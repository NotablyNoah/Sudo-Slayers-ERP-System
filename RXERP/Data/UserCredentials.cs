using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RXERP.Data;

namespace RXERP.Data
{
    /// <summary>
    /// Represents user credentials with email and password.
    /// </summary>
    public class UserCredentials
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Additional properties can be added as needed
        [Column("Role")]
        public string Role { get; set; } = "User"; // Default role is "User"

        [Column("CreateAt")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [NotMapped] // This property will not be mapped to the database
        [DataType(DataType.Password)]
        public string ConfirmPassword {get; set; } = string.Empty;
    }
}