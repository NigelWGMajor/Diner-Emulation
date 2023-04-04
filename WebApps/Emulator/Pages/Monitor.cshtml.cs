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
            // Ref: https://www.chartjs.org/docs/latest/
            var chartData = @"
            {
                type: 'bar',
                responsive: true,
                data:
                {
                    labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
                    datasets: [{
                        label: '# of Votes',
                        data: [12, 19, 3, 5, 2, 3],
                        backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)'
                            ],
                        borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                            ],
                        borderWidth: 1
                    }]
                },
                options: {
                    animation: true,
                    scales:
                    {
                        yAxes: [{
                            ticks:
                            {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            }";

            Chart = JsonConvert.DeserializeObject<ChartJs>(chartData);
            ChartJson = JsonConvert.SerializeObject(Chart, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }

        public void OnPost()
        {
            
        }
        public IActionResult OnPostSomething()
        {
            Chart.data.datasets[0].backgroundColor = new string[] 
            {"rgba(255, 255, 255, 0.2)",
            "rgba(255, 255, 255, 0.2)",
            "rgba(255, 255, 255, 0.2)",
            "rgba(255, 255, 255, 0.2)",
            "rgba(255, 255, 255, 0.2)",
            "rgba(255, 255, 255, 0.2)"
            };
            
            // return RedirectToPage("/Error");
            return Page();
        }
    }
}
