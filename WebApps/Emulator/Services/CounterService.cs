using Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Emulator.Services
{
    public interface ICounterService 
    { 
        int GetCounter();
    }

    public class CounterService : BackgroundService, ICounterService 
    {
        private readonly IHubContext<CounterHub> _hub;
        private static int _count;

        public CounterService(IHubContext<CounterHub> hub)
        {
            _hub = hub;
        }

        private int IncrementCounter()
        {
            return _count++;
        }
        public int GetCounter()
        {
            return _count;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hub.Clients.All.SendAsync(
                    "updateCounter",
                    IncrementCounter(),
                    cancellationToken: stoppingToken
                );
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
