using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Data // namespace declaration to match file path
{
    /// <summary>
    /// Database context class for the application.
    /// </summary>
    public class EmployeeData : DbContext
    {
        [Key]
        public int Employee_ID { get; set; }

        [Column]
        [Required]
        public string Last_Name { get; set; } = null!;

        [Column]
        [Required]
        public string First_Name { get; set; } = null!;

        [Column]
        public string? Department { get; set; }

        [Column]
        public string? Email { get; set; }

        [Column]
        public string? Phone { get; set; }

        [Column]
        public string? Address { get; set; }

        [Column("Employee_Role")]
        public string Role { get; set; } = "User"; // Default role is "user"

        [Column]
        public decimal Salary { get; set; }


    }
}
