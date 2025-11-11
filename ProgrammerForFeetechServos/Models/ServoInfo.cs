// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

namespace ProgrammerForFeetechServos.Models;

public class ServoInfo : IEquatable<ServoInfo>
{
    public byte Id { get; set; }
    public ushort ModelNumber { get; set; }
    public bool IsConnected { get; set; }

    public string ModelName => ModelNameLookup.GetName(ModelNumber);

    public override bool Equals(object? obj)
    {
        return Equals(obj as ServoInfo);
    }

    public bool Equals(ServoInfo? other)
    {
        return other is not null &&
               Id == other.Id &&
               ModelNumber == other.ModelNumber &&
               IsConnected == other.IsConnected;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
