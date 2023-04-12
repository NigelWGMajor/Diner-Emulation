using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Emulator.Services;
using Microsoft.AspNetCore.SignalR;

namespace Emulator.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
        public string PageName => "Index";
   
    public IndexModel(ILogger<IndexModel> logger)
    {
   
        _logger = logger;
        
    }
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
