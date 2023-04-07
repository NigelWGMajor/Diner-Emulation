using Microsoft.AspNetCore.SignalR;

namespace Emulator.Hubs;

public class EventLogHub : Hub
{
    public const string Url = "/event";
}