using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace Emulator.Services { }
/*
/// <summary>
/// https://stackoverflow.com/questions/12294296/list-with-limited-item
/// </summary>
/// <typeparam name="T"></typeparam>
public class Buffer<T> : Queue<T>
{
    public int? MaxCapacity { get; }
    public Buffer(int capacity) { MaxCapacity = capacity; }
    public int TotalItemsAddedCount { get; private set; }

    // public void Add(T newElement)
    // {
    //     // not thread safe 🤷‍
    //     if (Count == (MaxCapacity ?? -1)) Dequeue();
    //     Enqueue(newElement);
    //     TotalItemsAddedCount++;
    // }
}

public static class BufferExtensions
{
    public static PointSet AddNewRandomPoint(this Buffer<Point> buffer)
    {
        var now = DateTime.Now.AddMonths(buffer.TotalItemsAddedCount);
        var year = now.Year;
        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(now.Month);
        var points = new PointSet("", RandomNumberGenerator.GetInt32(1, 11), RandomNumberGenerator.GetInt32(1, 11), RandomNumberGenerator.GetInt32(1, 11), RandomNumberGenerator.GetInt32(1, 11));
        var point = new Point($"", RandomNumberGenerator.GetInt32(1, 11));
       // buffer.Add(point);
        return points;
    }
}
*/