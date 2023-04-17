using System.Globalization;
using System.Security.Cryptography;
using Emulator.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Emulator.Services;

public class ChartValueGenerator : BackgroundService
{
    private readonly IHubContext<ChartHub> _hub;
    //private readonly Buffer<Point> _data;

    public ChartValueGenerator(IHubContext<ChartHub> hub/*, Buffer<Point> data*/)
    {
        _hub = hub;
  //      _data = data;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            int i = RandomNumberGenerator.GetInt32(1, 11);
            await _hub.Clients.All.SendAsync(
                "addChartData",
                 new PointSet("", i, i+1, -i, 2*i),
                cancellationToken: stoppingToken
            );

            await Task.Delay(TimeSpan.FromSeconds(0.2), stoppingToken);
        }
    }
}