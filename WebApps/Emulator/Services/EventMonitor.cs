using Bogus;
using Emulator.Hubs;
using Emulator.Models.Log;
using Microsoft.AspNetCore.SignalR;
using Models.Common;
using WorkflowManager;
using RestaurantService;

namespace Emulator.Services
{
    public interface IUpdateable
    {
        void Update(List<LogItem> target);
    }

    public interface IEventMonitor
    {
        Task<IEnumerable<LogItem>> GetEvents();
        Task OrderFromTable();
        string AddEvent();

    }

    public class EventMonitor : BackgroundService, IEventMonitor
    {
        private WorkflowManager.Manager _manager;
        private RestaurantService.Restaurant _restaurant;

        private static string[] classes = { "log-failure", "log-retry", "log-success" };
        private List<LogItem> _events = new();
        private readonly IHubContext<EventLogHub> _hub;

        public EventMonitor(IHubContext<EventLogHub> hub)
        {
            _manager = new Manager();
            _restaurant = new Restaurant();
            _hub = hub;
            Randomizer.Seed = new Random(3897234);
        }

        private int _length = 20;
        private static Faker<LogItem> _userFaker = new Faker<LogItem>()
            .RuleFor(l => l.Content, (f, l) => f.Lorem.Lines(1))
            .RuleFor(l => l.EventClass, (f, l) => f.PickRandom(classes))
            .RuleFor(l => l.EventTime, DateTime.Now);

        public async Task OrderFromTable()
        {
            var table = _restaurant.GetNewTable();
            WorkflowManager.Models.WorkRequest request = new WorkflowManager.Models.WorkRequest
            {
                Initiator = table.Server.Name,
                Contact = table.Server.Name,
                ReceivedAt = DateTime.Now,
                Origin = table.TableNumber
            };
            await _manager.RequestWork(request, table);
        }

        public async Task<IEnumerable<LogItem>> GetEvents()
        {
            _events.AddRange(GenerateRandomEvents(1));
            if (_events.Count > _length)
            {
                _events.RemoveAt(0);
            }
            await Task.CompletedTask;
            return _events;
        }

        // private static IUpdateable? _remote;
        public delegate void UpdateRemoteEvents(List<LogItem> events);

        // public void SetRemote(IUpdateable remote)
        // {
        //     _remote = remote;
        // }

        public string AddEvent()
        {
            var x = GenerateRandomEvents(1);
            _events.AddRange(x);
            if (_events.Count > _length)
            {
                _events.RemoveAt(0);
            }
            return AsListItem(x[0]);
        }

        private string AsListItem(LogItem item)
        {
            return $"<li class='{item.EventClass}'> {item.EventTime:hh=MM-ss} --- {item.Content}</li>";
        }

        public List<LogItem> GenerateRandomEvents(int n)
        {
            return _userFaker.Generate(n);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hub.Clients.All.SendAsync(
                    "addEvent",
                    AddEvent(),
                    cancellationToken: stoppingToken
                );

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
