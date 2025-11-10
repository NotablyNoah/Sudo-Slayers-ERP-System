using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Pages
{
    public class HRMModel : PageModel
    {
        private readonly AppDbContent _db;

        public HRMModel(AppDbContent db)
        {
            _db = db;
        }

        // Bound collection shown in the table after filtering
        public List<EmployeeView> Employees { get; set; } = new List<EmployeeView>();

        // Bound model for Add/Edit form (names match DB columns)
        [BindProperty]
        public EmployeeInput Input { get; set; } = new EmployeeInput();

        // Department filter binding
        [BindProperty]
        public string? SelectedDepartment { get; set; } = "All";

        // Summary values (computed from Employees)
        public int TotalEmployees => Employees?.Count ?? 0;
        public int DepartmentCount => Employees?.Select(e => e.Department ?? "").Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).Count() ?? 0;
        public decimal AvgSalary => Employees != null && Employees.Count > 0 ? Math.Round(Employees.Average(e => e.Salary), 2) : 0m;
        public decimal MonthlyPayroll => Math.Round((Employees?.Sum(e => e.Salary) ?? 0m) / 12m, 2);

        // GET: load list, optionally load a vendor for editing when id is supplied
        public async Task<IActionResult> OnGetAsync(int? editId)
        {
            // Access control (same behavior as before)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "HR")
            {
                TempData["ErrorMessage"] = "Access denied. Admins/HR only.";
                return RedirectToPage("/Index");
            }

            await LoadEmployeesAsync();

            // If edit requested, populate Input for editing
            if (editId.HasValue)
            {
                var existing = await _db.Set<EmployeeData>().FindAsync(editId.Value);
                if (existing != null)
                {
                    Input = new EmployeeInput
                    {
                        Employee_ID = existing.Employee_ID,
                        First_Name = existing.First_Name,
                        Last_Name = existing.Last_Name,
                        Department = existing.Department,
                        Email = existing.Email,
                        Phone = existing.Phone,
                        Address = existing.Address,
                        Role = existing.Role,
                        Salary = existing.Salary
                    };
                }
                else
                {
                    TempData["Error"] = "Employee not found.";
                }
            }

            // Apply department filter if set in query (SelectedDepartment is bound)
            if (!string.IsNullOrEmpty(SelectedDepartment) && !string.Equals(SelectedDepartment, "All", StringComparison.OrdinalIgnoreCase))
            {
                Employees = Employees.Where(e => string.Equals(e.Department ?? "", SelectedDepartment, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Page();
        }

        // POST: apply filter (re-render page with filter)
        public async Task<IActionResult> OnPostFilterAsync()
        {
            await LoadEmployeesAsync();
            if (!string.IsNullOrEmpty(SelectedDepartment) && !string.Equals(SelectedDepartment, "All", StringComparison.OrdinalIgnoreCase))
            {
                Employees = Employees.Where(e => string.Equals(e.Department ?? "", SelectedDepartment, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return Page();
        }

        // POST: Add or Update employee
        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync();
                if (!string.IsNullOrEmpty(SelectedDepartment) && !string.Equals(SelectedDepartment, "All", StringComparison.OrdinalIgnoreCase))
                {
                    Employees = Employees.Where(e => string.Equals(e.Department ?? "", SelectedDepartment, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                return Page();
            }

            if (Input.Employee_ID == 0)
            {
                // Create new entity and persist
                var entity = new EmployeeData
                {
                    First_Name = Input.First_Name!,
                    Last_Name = Input.Last_Name!,
                    Department = Input.Department,
                    Email = Input.Email,
                    Phone = Input.Phone,
                    Address = Input.Address,
                    Role = Input.Role,
                    Salary = Input.Salary
                };

                _db.Set<EmployeeData>().Add(entity);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Employee added.";
            }
            else
            {
                // Update existing
                var existing = await _db.Set<EmployeeData>().FindAsync(Input.Employee_ID);
                if (existing == null)
                {
                    TempData["Error"] = "Employee not found.";
                }
                else
                {
                    existing.First_Name = Input.First_Name!;
                    existing.Last_Name = Input.Last_Name!;
                    existing.Department = Input.Department;
                    existing.Email = Input.Email;
                    existing.Phone = Input.Phone;
                    existing.Address = Input.Address;
                    existing.Role = Input.Role;
                    existing.Salary = Input.Salary;

                    _db.Set<EmployeeData>().Update(existing);
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Employee updated.";
                }
            }

            return RedirectToPage();
        }

        // POST: Delete employee
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var existing = await _db.Set<EmployeeData>().FindAsync(id);
            if (existing != null)
            {
                _db.Set<EmployeeData>().Remove(existing);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Employee deleted.";
            }
            else
            {
                TempData["Error"] = "Employee not found.";
            }
            return RedirectToPage();
        }

        // Helper: load employees from DB into view DTOs and compute summaries
        private async Task LoadEmployeesAsync()
        {
            // Query EmployeeData rows and project to a view model where property names match DB schema
            Employees = await _db.Set<EmployeeData>()
                .AsNoTracking()
                .OrderBy(e => e.Last_Name)
                .ThenBy(e => e.First_Name)
                .Select(e => new EmployeeView
                {
                    Employee_ID = e.Employee_ID,
                    Last_Name = e.Last_Name,
                    First_Name = e.First_Name,
                    Department = e.Department,
                    Email = e.Email,
                    Phone = e.Phone,
                    Address = e.Address,
                    Role = e.Role,
                    Salary = e.Salary
                })
                .ToListAsync();

            // If no employees found, make Employees an empty list to avoid null references
            Employees = Employees ?? new List<EmployeeView>();
        }

        // View DTO (read-only projection) — fields follow EmployeeData column names
        public class EmployeeView
        {
            public int Employee_ID { get; set; }
            public string Last_Name { get; set; } = null!;
            public string First_Name { get; set; } = null!;
            public string? Department { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string Role { get; set; } = "User";
            public decimal Salary { get; set; }
        }

        // Input model used for binding the form — names match DB columns
        public class EmployeeInput
        {
            public int Employee_ID { get; set; } = 0;

            [Required(ErrorMessage = "Last name is required")]
            [Display(Name = "Last Name")]
            public string? Last_Name { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [Display(Name = "First Name")]
            public string? First_Name { get; set; }

            [Display(Name = "Department")]
            public string? Department { get; set; }

            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone(ErrorMessage = "Invalid phone number")]
            [Display(Name = "Phone")]
            public string? Phone { get; set; }

            [Display(Name = "Address")]
            public string? Address { get; set; }

            [Display(Name = "Role")]
            public string Role { get; set; } = "User";

            [Range(0, 100000000, ErrorMessage = "Salary must be non-negative")]
            [Display(Name = "Salary")]
            public decimal Salary { get; set; }
        }
    }
}