using Emulator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emulator.Pages
{
    public class SettingsModel : PageModel
    {
        public string PageName => "Settings";

        public string Message { get; set; }
        private readonly ILogger<SettingsModel> _logger;
        private int _sliderValue;

        [BindProperty(SupportsGet = true)]
        public DateTime DateValue1 { get; set;} = DateTime.Now;

        [BindProperty(SupportsGet = true)]

        public int SliderValue1 { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SliderValue2 { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SliderValue3 { get; set; }

        public SettingsModel(ILogger<SettingsModel> logger, IStorage storage)
        {
            _logger = logger;
            Message = storage.GetTime();
        }

        public void OnGet()
        {
            SliderValue1 = 42;
            SliderValue3 = 100 - 42;

        }

        public IActionResult OnPostSomething()
        {
            return RedirectToPage("/Error");
            //    return Page();
        }

        public void OnPost()
        {
        }
    }
}
