
using Microsoft.AspNetCore.Mvc;
using Emulator.Services;
using System.Threading.Tasks;

namespace ViewComponents
{
    public class CounterViewComponent : ViewComponent
    {
        private readonly ICounterService _counterService;
        public CounterViewComponent(ICounterService counterService)
        {
            _counterService = counterService;
        }

        // You can have zero or more parameters, and they may be optional if >= net6.0
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var x = _counterService.GetCounter();
            ViewData["Count"] = x;
            return View(); // if empty, this passes the default view - in this case the IndexModel.
        }
    }
}
