using Bogus;
using Emulator.Hubs;
using Emulator.Models.Log;
using Microsoft.AspNetCore.SignalR;
using WorkflowManager;
using WorkflowManager.Models;
using RestaurantService;
using Azure.Core;
using System.Text;
using Person = Bogus.Person;

namespace Emulator.Services
{
    public interface IUpdateable
    {
        void Update(List<EventLogItem> target);
    }

    public interface IEventMonitor
    {
        Task OrderFromTable();

        Task ClaimResponsibility();
        Task<string> AddEvent();

        Task<IOperable> GetNextOperationAsync();

        Task NotifyResultAsync(IOperable attempt);

    }

    public class EventMonitor : BackgroundService, IEventMonitor
    {
        private WorkflowManager.IFlowManager _manager;
        private RestaurantService.Restaurant _restaurant;
        private Faker _faker = new Faker();
        private static string[] classes = { "log-red", "log-yellow", "log-green", "log-blue" };
        private static string[] executors = { "Chef Ramsey", "Chef Blaise", "Chef Nyesha Arrington" };
        private List<EventLogItem> _events = new();
        private readonly IHubContext<EventLogHub> _eventHub;
        private readonly IHubContext<ChartHub> _chartHub;
        public EventMonitor(IHubContext<EventLogHub> eventHub, IHubContext<ChartHub> chartHub)
        {
            _manager = new FlowManager(Environment.GetEnvironmentVariable("NIX_DB"));
            _restaurant = new Restaurant();
            _eventHub = eventHub;
            _chartHub = chartHub;
            Randomizer.Seed = new Random(3897234);
        }

        private int _length = 20;
        private static Faker<Person> _userFaker = new Faker<Person>()
            .RuleFor(p => p.FirstName, (p, f) => $"{f.FirstName} {f.LastName.Substring(0, 1)}");

        public async Task OrderFromTable()
        {
            var table = _restaurant.GetNewTable();
            await _manager.ActivateAsync(table);
        }
        private Stack<string> busyChefs = new Stack<string>();
        public async Task ClaimResponsibility()
        {
            ////   int x = await _manager.GetNextAsync();
            //if (x ==0)
            //{
            // nothing to claim
            //OrderFromTable();
            //_manager.ClaimResponsibilityAsync(executors[0]);
            //}
        }
        public async Task<IOperable> GetNextOperationAsync()
        {
            var x = await _manager.ProceedAsync();
            //if (x == null)
            //{
            //    ClaimResponsibility();
            //    x = await _manager.GetNextOperationAsync(executors[0]);
            //}
            return x;
        }
        public async Task NotifyResultAsync(IOperable operable)
        {
            var result = _manager.UpdateAsync(operable);
            // if (result.IsCanceled) ...
        }


        public async Task<string> AddEvent()
        {
            var x = await _manager.GetLog(20);
            return AsListItems(x);
        }
        private static int _counter = 1;
        private static string[] _eventClasses = { "log-blue", "log-green", "log-yellow", "log-red" };
        private string AsListItems(IEnumerable<FlowLogItem> items)
        {

            StringBuilder sb = new StringBuilder();
            if (items != null)
                foreach (var item in items)
                {
                    sb.Append($"<li class='{_eventClasses[item.Severity]}'> {item.EventTime:t} {item.Content}</li>");
                }
            return sb.ToString();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var x = await AddEvent();
                if (!String.IsNullOrEmpty(x))
                {

                    await _eventHub.Clients.All.SendAsync(
                        "addEvent",
                        x,
                        cancellationToken: stoppingToken
                    );
                }
                if (_counter++ % 5 == 0)
                {
                    var stats = await _manager.GetMetrics();
                    var data = new PointSet($"{_counter++}", new int[] { stats.ActivationsPending, stats.ActivationsActive, stats.ActivationsFailed, stats.ActivationsSucceeded });
                    await _chartHub.Clients.All.SendAsync(
                        "addChartData",
                         data,
                        cancellationToken: stoppingToken
                    );
                }
                await Task.Delay(TimeSpan.FromSeconds(0.5), stoppingToken);
            }
        }
    }
}

