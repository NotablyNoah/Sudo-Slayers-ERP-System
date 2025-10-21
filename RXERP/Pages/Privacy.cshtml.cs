using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RXERP.Pages;

public class PrivacyModel : PageModel
{
    [BindProperty] public double num1 { get; set; }
    [BindProperty] public double num2 { get; set; }
    [BindProperty] public string? operation { get; set; }

    public double? result { get; set; }

    public IActionResult OnPost()
    {
        switch (operation)
        {
            case "+":
                result = num1 + num2;
                break;
            case "-":
                result = num1 - num2;
                break;
            case "x":
                result = num1 * num2;
                break;
            case "/":
                if (num2 != 0)
                {
                    result = num1 / num2;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cannot divide by zero.");
                }
                break;
            default:
                ModelState.AddModelError(string.Empty, "Invalid operation.");
                break;
        }
        return Page();
    }
    public void OnGet()
    {
    }
}

