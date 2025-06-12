using Auth.Dtos.User;
using Auth.Models;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class UserFilterParamsValidator: AbstractValidator<UsersFilterParams>
{
    private static readonly string[] _allowedSortDirs = ["asc", "desc"];
    private static readonly string[] _allowedSortFields = ["firstName", "lastName", "createdAt"];
    public UserFilterParamsValidator()
    {
        RuleFor(x => x.Email)
          .EmailAddress()
          .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x)
          .Must(x => !(x.CreatedAfter.HasValue && x.CreatedBefore.HasValue && x.CreatedAfter > x.CreatedBefore))
          .WithName(nameof(User.CreatedAt))
          .WithMessage("'createdAfter' must be on or before 'createdBefore'");

        RuleFor(x => x.SortField)
          .Must(s => s == null || _allowedSortFields.Contains(s))
          .WithName(nameof(UsersFilterParams.SortField))
          .WithMessage("Invalid sort field");

        RuleFor(x => x.SortDir)
          .Must(s => s == null || _allowedSortDirs.Contains(s))
          .WithName("sortDirection")
          .WithMessage("Invalid sort direction");
    }
}