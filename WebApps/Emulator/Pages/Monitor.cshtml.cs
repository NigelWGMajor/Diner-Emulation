using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Emulator.Models.Chart;
using Emulator.Services;

namespace Emulator.Pages
{
    public class MonitorModel : PageModel
    {    
        
	    public string PageName => "Monitor";
        public static ChartJs Chart { get; set; } = new ChartJs();
        public string ChartJson { get; set; } = "";
        private readonly ILogger<MonitorModel> _logger;
        private readonly IEventMonitor _eventMonitor;
        public MonitorModel(ILogger<MonitorModel> logger, IEventMonitor eventMonitor)
        {
            _logger = logger;
            _eventMonitor = eventMonitor;
        }
        public void OnGet()
        {
           
        }

        public void OnPost()
        {
            
        }
        public void OnPostSomething()
        {
            ;
            
        }
    }
}
