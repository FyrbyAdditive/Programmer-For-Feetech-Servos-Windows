// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

namespace ProgrammerForFeetechServos.Models;

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Error
}

public class ConnectionStateInfo
{
    public ConnectionState State { get; set; }
    public string? ErrorMessage { get; set; }

    public static ConnectionStateInfo Disconnected() => new() { State = ConnectionState.Disconnected };
    public static ConnectionStateInfo Connecting() => new() { State = ConnectionState.Connecting };
    public static ConnectionStateInfo Connected() => new() { State = ConnectionState.Connected };
    public static ConnectionStateInfo Error(string message) => new() { State = ConnectionState.Error, ErrorMessage = message };

    public override bool Equals(object? obj)
    {
        if (obj is ConnectionStateInfo other)
        {
            return State == other.State && ErrorMessage == other.ErrorMessage;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(State, ErrorMessage);
    }

    public static bool operator ==(ConnectionStateInfo? left, ConnectionStateInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ConnectionStateInfo? left, ConnectionStateInfo? right)
    {
        return !(left == right);
    }
}
