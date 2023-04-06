using Bogus;
using Emulator.Models.Log;

namespace Emulator.Services
{
    public interface IEventMonitor
    {
       Task<IEnumerable<LogItem>> GetEvents();
    }
    public class EventMonitor : IEventMonitor
    {
        private static string[] classes = { "log-failure", "log-retry", "log-success" };
        private List<LogItem> _events = new();

        public EventMonitor()
        {
            Randomizer.Seed = new Random(3897234);
        }

        private int _length = 20;
        private static Faker<LogItem> _userFaker = new Faker<LogItem>()
            .RuleFor(l => l.Content, (f, l) => f.Lorem.Lines(1))
            .RuleFor(l => l.EventClass, (f, l) => f.PickRandom(classes))
            .RuleFor(l => l.EventTime, DateTime.Now);

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

        private List<LogItem> GenerateRandomEvents(int n)
        {
            return _userFaker.Generate(n);
        }
    }
}
