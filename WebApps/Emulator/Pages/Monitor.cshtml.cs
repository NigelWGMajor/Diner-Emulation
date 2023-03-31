using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emulator.Pages
{
    public class MonitorModel : PageModel
    {        public string PageName => "Monitor";
        private readonly ILogger<MonitorModel> _logger;

        public MonitorModel(ILogger<MonitorModel> logger)
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
