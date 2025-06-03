using System.Net;

namespace Shared.Exceptions;

public class ConflictException : Exception
{
    public string? Field { get; set; }
    public string? Value { get; set; }
    public ConflictType ConflictType { get; set; }

    public ConflictException(string message, ConflictType conflictType = ConflictType.General): base(message)
    {
        ConflictType = conflictType;
    }

    public ConflictException(string field, string value, ConflictType conflictType): base(GenerateMessage(field, value, conflictType))
    {
        Field = field;
        Value = value;
        ConflictType = conflictType;
    }

    private static string GenerateMessage(string field, string value, ConflictType conflictType)
    {
        return conflictType switch
        {
            ConflictType.AlreadyExists => $"The {field.ToLower()} '{value}' is already in use.",
            ConflictType.InUse => $"The {field.ToLower()} '{value}' cannot be deleted because it is currently in use.",
            ConflictType.AlreadyAssigned => $"The {field.ToLower()} '{value}' is already assigned.",
            ConflictType.NotAssigned => $"The {field.ToLower()} '{value}' is not assigned and cannot be removed.",
            ConflictType.Duplicate => $"Duplicate {field.ToLower()} '{value}' detected.",
            _ => $"Conflict with {field.ToLower()} '{value}'."
        };
    }
}

public enum ConflictType { General, AlreadyExists, InUse, NotAssigned, AlreadyAssigned, Duplicate }