using System;

namespace First.Contracts;

public record LogEntryDescription
{
    public DateTimeOffset? TimeStamp { get; set; }
    public bool Succeeded { get; set; }
    public string? LogEntryName { get; set; }
}