using FluentValidation.Results;

namespace Shared.Utils;

public static class FluentValidationUtils
{
    public static List<(string field, string message)> HandleValidationErrors(ValidationResult result)
    {
        return result.Errors
            .Select(e => (field: GeneralUtils.PascalToCamel(e.PropertyName), message: e.ErrorMessage))
            .ToList();
    }
}