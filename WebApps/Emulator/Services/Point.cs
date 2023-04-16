namespace Emulator.Services;

public record Point(string Label, int Value);
public record PointSet (string Label, params int[] Values);