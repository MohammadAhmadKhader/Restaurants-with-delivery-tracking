using FluentValidation.Results;

namespace Shared.Utils;

public static class FluentValidationUtils
{
    public static List<(string field, string message)> HandleValidationErrors(ValidationResult result)
    {
        return result.Errors
            .Select(e => (field: ToCamelCase(e.PropertyName), message: e.ErrorMessage))
            .ToList();
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLower(name[0]) + name.Substring(1);
    }
}