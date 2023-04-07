using Microsoft.AspNetCore.SignalR;

namespace Emulator.Hubs
{
    public class CounterHub : Hub
    {
        public const string Url = "/counter";
        public async Task SetCount(int count)
        {
            await Clients.All.SendAsync("getCount", count);
        }
    }
}