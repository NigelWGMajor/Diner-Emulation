using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Emulator.Models.Chart;
using Emulator.Models.Log;
using Emulator.Services;
using Microsoft.JSInterop;
using System.Threading;
using Models.Common;
using Emulator.Pages.Shared;

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
        public List<EventLogItem> Events { get; set; } = new List<EventLogItem>();
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
             _eventMonitor.OrderFromTable();
        }
        public void OnPostClaimResponsibility()
        {
            _eventMonitor.ClaimResponsibility();
        }
        public async Task OnPostTrySucceed()
        {
            Attempt attempt = await _eventMonitor.GetNextOperationAsync();
            if (attempt != null)
            {
                attempt.Outcome = "Succeeded";
                // pretend to do something good!
                _eventMonitor.NotifyResultAsync(attempt);
            }
        }
        public async Task OnPostTryFail()
        {
            Attempt attempt = await _eventMonitor.GetNextOperationAsync();
            if (attempt == null) return;
            attempt.Outcome= "Failed";
            // pretend to do something bad!
            _eventMonitor.NotifyResultAsync(attempt);
        }
    }
}
