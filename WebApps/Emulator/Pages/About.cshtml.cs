using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emulator.Pages
{
    public class AboutModel : PageModel
    {
        public string PageName => "About";
        private readonly ILogger<AboutModel> _logger;

        public AboutModel(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            
        }
        public void OnPost()
        {
            
        }
        public IActionResult OnPostSomething()
        {
            return RedirectToPage("/Error");
        //    return Page();
        }
    }
}