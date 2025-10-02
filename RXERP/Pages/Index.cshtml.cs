using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RXERP.Pages;

public class IndexModel : PageModel
{
    // Properties to bind the username and password from the form
    [BindProperty] public string username { get; set; } = string.Empty;
    [BindProperty] public string password { get; set; } = string.Empty;

    // This method is called when the page is accessed via a GET request  
    public void OnGet()
    {

    }

    // This method is called when the form is submitted via a POST request
    public IActionResult OnPost()
    {
        // Here you can add your authentication logic
        if (username == "admin@erp.com" && password == "p") // Example check
        {
            // Redirect to a different page on successful login
            return RedirectToPage("/HRM");
        }
        else
        {
            // Stay on the same page and perhaps show an error message
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
