using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Charts_RazorPage.Models.Chart;

namespace Emulator.Pages
{
    public class MonitorModel : PageModel
    {    
        
	    public string PageName => "Monitor";
        public static ChartJs Chart { get; set; } = new ChartJs();
        public string ChartJson { get; set; } = "";
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
            return Page();
        }
    }
}
