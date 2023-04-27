using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Emulator.Models.Chart;
using Emulator.Models.Log;
using Emulator.Services;
using Microsoft.JSInterop;
using System.Threading;
using Emulator.Pages.Shared;
using WorkflowManager.Models;
using WorkflowManager;

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
        private static Queue<IOperable> _lastAttempts = new Queue<IOperable>();
        public async Task OnPostTryNext()
        {
            IOperable attempt = await _eventMonitor.GetNextOperationAsync();
            if (attempt != null)
                _lastAttempts.Enqueue(attempt);
        }
        public async Task OnPostTrySucceed()
        {
            IOperable attempt;
            if (_lastAttempts.Count > 0)
            {
                attempt = _lastAttempts.Dequeue();
            }
            else
            {
                attempt = await _eventMonitor.GetNextOperationAsync();
            }
            if (attempt != null)
            {
                attempt.Completion = CompletionState.Succeeded;
                _eventMonitor.NotifyResultAsync(attempt);
            }
        }
        public async Task OnPostTryFail()
        {
            IOperable attempt;
            if (_lastAttempts.Count > 0)
            {
                attempt = _lastAttempts.Dequeue();
            }
            else
            {
                attempt = await _eventMonitor.GetNextOperationAsync();
            }
            if (attempt == null) return;
            attempt.Completion = CompletionState.Failed;
            _eventMonitor.NotifyResultAsync(attempt);
        }
    }
}
