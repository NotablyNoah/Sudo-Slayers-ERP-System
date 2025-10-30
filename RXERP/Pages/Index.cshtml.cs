using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;   
namespace RXERP.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContent _db; // Database context

    // Constructor to initialize the database context
    public IndexModel(AppDbContent db)
    {
        _db = db;
    }

    [BindProperty]
    public UserCredentials UserCredentials { get; set; } = new UserCredentials();

    // This method is called when the page is accessed via a GET request  
    public void OnGet()
    {

    }

    // This method is called when the form is submitted via a POST request
    // public IActionResult OnPost() is a syncronous method that handles form submissions at the same time
    // user login processing will be using an async method
    // allowing the server to handle other requests while waiting for database operations to complete
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Simple validation logic
        if (string.IsNullOrWhiteSpace(UserCredentials.Email) || string.IsNullOrWhiteSpace(UserCredentials.Password))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return Page();
        }

        // check credentials against the database
        var user = await _db.UserCredentials
            .FirstOrDefaultAsync(u => u.Email == UserCredentials.Email && u.Password == UserCredentials.Password);

        if (user != null && user.Role == "Admin")
        {
            // User is authenticated and is an admin
            return RedirectToPage("/HRM");
        }
        else if (user != null && user.Role == "User")
        {
            // User is authenticated and is a regular user
            return RedirectToPage("/Privacy");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }
    }
}
