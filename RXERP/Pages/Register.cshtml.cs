using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RXERP.Data;

namespace RXERP.Pages;

public class RegisterModel : PageModel
{
    // connect to AppDbContent database context
    private readonly AppDbContent _db;

    public RegisterModel(AppDbContent db)
    {
        _db = db;
    }

    [BindProperty]
    public UserCredentials UserCredentials { get; set; } = new UserCredentials();

    // This method is called when the page is accessed via a GET request
    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //check if Email already exists
        var existingUser = await _db.UserCredentials
            .FirstOrDefaultAsync(u => u.Email == UserCredentials.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "Email is already registered.");
            return Page();
        }
        
        //check if Password matches with ConfirmPassword
        if (UserCredentials.Password != UserCredentials.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match.");
            return Page();
        }

        // Add the new user to the database
        _db.UserCredentials.Add(UserCredentials);
        await _db.SaveChangesAsync();

        // Redirect to the login page after successful registration
        return RedirectToPage("/Index");
    }
}