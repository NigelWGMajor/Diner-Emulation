using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Emulator.Models.Chart;
using Emulator.Models.Log;
using Emulator.Services;
using Microsoft.JSInterop;
using System.Threading;

namespace Emulator.Pages
{
    public interface IUpdatable<T> 
    { 
        void SetRemote(T element);
    }
    public class MonitorModel : PageModel
    {    
        private readonly IJSRuntime _js;

        [BindProperty]
        public List<LogItem> Events { get; set; } = new List<LogItem>();
	    public string PageName => "Monitor";
        public static ChartJs Chart { get; set; } = new ChartJs();
        public string ChartJson { get; set; } = "";
        private readonly ILogger<MonitorModel> _logger;
        private readonly IEventMonitor _eventMonitor;
        public MonitorModel(ILogger<MonitorModel> logger, IEventMonitor eventMonitor, IJSRuntime js)
        {
            _js = js;
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
