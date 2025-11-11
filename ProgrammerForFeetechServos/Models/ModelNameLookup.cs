// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

namespace ProgrammerForFeetechServos.Models;

public static class ModelNameLookup
{
    private static readonly Dictionary<ushort, string> ModelNames = new()
    {
        { 777, "STS 3215" },
        { 2825, "STS 3250" },
        // Add more mappings as needed
    };

    public static string GetName(ushort modelNumber)
    {
        return ModelNames.TryGetValue(modelNumber, out var name) 
            ? name 
            : $"Unknown Model {modelNumber}";
    }
}
