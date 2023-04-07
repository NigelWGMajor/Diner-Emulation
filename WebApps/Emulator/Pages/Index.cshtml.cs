using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Emulator.Services;
using Microsoft.AspNetCore.SignalR;

namespace Emulator.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
        public string PageName => "Index";
    private readonly ICounterService _counterService;
    public IndexModel(ILogger<IndexModel> logger, ICounterService counterService)
    {
        _counterService = counterService;
        _logger = logger;
        
    }
    public int Count { get => _counterService.GetCounter(); }  
    public void OnGet()
    {
         ;
    }

    public void OnPost()
    {
        ;
    }
    public IActionResult OnPostSomething()
    {
        return RedirectToPage("/Error");
    //    return Page();
    }
}
