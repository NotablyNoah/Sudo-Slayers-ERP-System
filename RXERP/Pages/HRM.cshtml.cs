using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace RXERP.Pages;

public class HRMModel : PageModel
{
    //edit later
    //public List<Employee> Employees { get; set; } = new List<Employee>();
    public async Task<IActionResult> OnGetAsync()
    {
        //create a variable save UserRole values from session
        var userRole = HttpContext.Session.GetString("UserRole");
        //check if userRole is null or not equal to "Admin"
        if (userRole != "Admin")
        {
            //if not admin redirect to Index page
            TempData["ErrorMessage"] = "Access denied. Admins only.";
            return RedirectToPage("/Index");
        }

        //Sprint 3: Display employee data here later
        //it will be a list of stored employees from the database

        return Page();
    }
}