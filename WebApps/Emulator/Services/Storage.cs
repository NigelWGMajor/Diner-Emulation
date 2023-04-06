namespace Emulator.Services;

public interface IStorage
{
    string GetTime();
}
public class Storage : IStorage
{
    public string GetTime() => $"{DateTime.Now:hh:mm:ss}";
}