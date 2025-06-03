using FluentValidation;
using FluentValidation.Results;

namespace Shared.Exceptions;

public class AppValidationException: Exception
{
    public string Field { get; set; } = default!;
    public string Value { get; set; } = default!;
    public AppValidationException(string field, string message): base()
    {
        var failure = new ValidationFailure(
            propertyName: field, 
            errorMessage: message
        );

        throw new ValidationException([ failure ]);
    }
}