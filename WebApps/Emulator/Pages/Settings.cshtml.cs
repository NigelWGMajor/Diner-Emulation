using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emulator.Pages
{
    public class SettingsModel : PageModel
    {
             private readonly ILogger<SettingsModel> _logger;

        public SettingsModel(ILogger<SettingsModel> logger)
        {
            _logger = logger;
        }    
        public void OnGet()
        {

        }
        public IActionResult OnPostSomething()
        {
            return RedirectToPage("/Error");
        //    return Page();
        }
    }
}
