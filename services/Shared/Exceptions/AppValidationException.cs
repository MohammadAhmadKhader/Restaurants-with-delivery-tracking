using FluentValidation;
using FluentValidation.Results;

namespace Shared.Exceptions;

public class AppValidationException : ValidationException
{
    public AppValidationException(string field, string message) : base([new ValidationFailure(field, message)])
    { }
    
    public AppValidationException(IEnumerable<ValidationFailure> failures): base(failures)
    { }
}