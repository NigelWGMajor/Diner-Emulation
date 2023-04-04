using Microsoft.AspNetCore.SignalR;

namespace Emulator.Hubs;

public class ChartHub : Hub
{
    public const string Url = "/chart";
}