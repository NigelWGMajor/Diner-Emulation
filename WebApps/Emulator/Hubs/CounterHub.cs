using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    public class CounterHub : Hub
    {
        public const string Url = "/counter";

    }
}