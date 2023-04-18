using Microsoft.AspNetCore.Mvc;
using Emulator.Services;
using System.Threading.Tasks;
using Emulator.Models.Log;

namespace Emulator.ViewComponents
{

    // expect to find the defined component in Pages\Shared\Components\XXX\default.cs 
    public class EventLogViewComponent : ViewComponent
    {
        private IEventMonitor _eventMonitor;
        // inject any required services here:
        public EventLogViewComponent(IEventMonitor eventMonitor)
        {
           _eventMonitor = eventMonitor;
        }
        // You can have zero or more parameters, and they may be optional if >= net6.0
        public async Task<IViewComponentResult> InvokeAsync()
        {
           // IEnumerable<Models.Log.EventLogItem> events = await _eventMonitor.GetEvents();
            return View(new List<EventLogItem>());
        }
    }
}