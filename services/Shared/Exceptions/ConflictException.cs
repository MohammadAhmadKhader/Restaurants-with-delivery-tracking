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
        var name = field.ToLowerInvariant();
        return conflictType switch
        {
            ConflictType.InUse              => $"The {name} '{value}' cannot be deleted because it is currently in use.",
            ConflictType.AlreadyAssigned    => $"The {name} '{value}' is already assigned.",
            ConflictType.NotAssigned        => $"The {name} '{value}' is not assigned and cannot be removed.",
            ConflictType.Duplicate          => $"Duplicate {name} '{value}' detected.",
            _                               => $"Conflict with {name} '{value}'."
        };
    }
}

public enum ConflictType { General, InUse, NotAssigned, AlreadyAssigned, Duplicate }