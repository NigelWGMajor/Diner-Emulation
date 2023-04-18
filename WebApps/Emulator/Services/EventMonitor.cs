using Bogus;
using Emulator.Hubs;
using Emulator.Models.Log;
using Microsoft.AspNetCore.SignalR;
using Models.Common;
using WorkflowManager;
using RestaurantService;
using Azure.Core;
using System.Text;
using Person = Bogus.Person;
using System.Xml;
using System.Diagnostics.Metrics;

namespace Emulator.Services
{
    public interface IUpdateable
    {
        void Update(List<EventLogItem> target);
    }

    public interface IEventMonitor
    {
        //Task<IEnumerable<EventLogItem>> GetEvents();
        Task OrderFromTable();

        Task ClaimResponsibility();
        string AddEvent();

        Task<Attempt> GetNextOperationAsync();

        Task NotifyResultAsync(Attempt attempt);

    }

    public class EventMonitor : BackgroundService, IEventMonitor
    {
        private WorkflowManager.Manager _manager;
        private RestaurantService.Restaurant _restaurant;
        private Faker _faker = new Faker();
        private static string[] classes = { "log-red", "log-yellow", "log-green", "log-blue" };
        private static string[] executors = { "Chef Rachael Ray", "Chef Ramsey", "Chef Balise", "Chef Nyesha Arrington" };
        private List<EventLogItem> _events = new();
        private readonly IHubContext<EventLogHub> _hub;

        public EventMonitor(IHubContext<EventLogHub> hub)
        {
            _manager = new Manager();
            _restaurant = new Restaurant();
            _hub = hub;
            Randomizer.Seed = new Random(3897234);
        }

        private int _length = 20;
        private static Faker<Person> _userFaker = new Faker<Person>()
            .RuleFor(p => p.FirstName, (p, f) => $"{f.FirstName} {f.LastName.Substring(0, 1)}");

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
            await _manager.RequestDeliverable(request, table);
        }
        private Stack<string> busyChefs = new Stack<string>();
        public async Task ClaimResponsibility()
        {
            int x = await _manager.ClaimResponsibility(executors[0]);
        }
        public async Task<Attempt> GetNextOperationAsync()
        {
            return await _manager.GetNextOperationAsync(executors[0]);
        }
        public async Task NotifyResultAsync(Attempt attempt)
        {
            await _manager.NotifyResultAsync(attempt);
        }

        //public async Task<IEnumerable<EventLogItem>> GetEvents()
        //{
        //    _events.AddRange(GenerateRandomEvents(1));
        //    if (_events.Count > _length)
        //    {
        //        _events.RemoveAt(0);
        //    }
        //    await Task.CompletedTask;
        //    return _events;
        //}

        // private static IUpdateable? _remote;
        public delegate void UpdateRemoteEvents(List<EventLogItem> events);

        // public void SetRemote(IUpdateable remote)
        // {
        //     _remote = remote;
        // }

        public string AddEvent()
        {
            var x = _manager.GetEvents();
            //var x = GenerateRandomEvents(1);
            EventLogItem item = x.FirstOrDefault(); ;
            foreach (var i in x)
            {
                item = i;
                _events.Add(i);
            }
            if (_events.Count > _length)
            {
                _events.RemoveAt(0);
            }
            return AsListItems(x);
        }
        private static int _counter = 1;
        private string AsListItems(IEnumerable<EventLogItem> items)
        {
            StringBuilder sb = new StringBuilder();
            if (items != null)
                foreach (var item in items)
                {
                    sb.Append($"<li class='{item.EventClass}'> {item.Content}</li>");
                }
            return sb.ToString();
        }

        //public List<EventLogItem> GenerateRandomEvents(int n)
        //{
        //    return _userFaker.Generate(n);
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var x = AddEvent();
                if (!String.IsNullOrEmpty(x))
                {

                    await _hub.Clients.All.SendAsync(
                        "addEvent",
                        x,
                        cancellationToken: stoppingToken
                    );
                }

                await Task.Delay(TimeSpan.FromSeconds(0.5), stoppingToken);
            }
        }
    }
}
