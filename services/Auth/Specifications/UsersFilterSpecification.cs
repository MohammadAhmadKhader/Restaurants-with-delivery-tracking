using System.Linq.Expressions;
using Auth.Dtos.User;
using Auth.Models;
using Shared.Specifications;

namespace Auth.Specifications;

public class UsersFilterSpecification : BaseSpecification<User>
{
    public UsersFilterSpecification(UsersFilterParams filterParams) : base()
    {
        Expression<Func<User, bool>> spec = PredicateBuilder.True<User>();

        if (!string.IsNullOrWhiteSpace(filterParams.Email))
        {
            spec = spec.And((u) => u.NormalizedEmail == filterParams.Email.ToUpper());
        }

        if (!string.IsNullOrWhiteSpace(filterParams.FirstName))
        {
            spec = spec.And((u) => u!.FirstName!.Contains(filterParams.FirstName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.LastName))
        {
            spec = spec.And((u) => u!.LastName!.Contains(filterParams.LastName.ToLower()));
        }

        if (filterParams.CreatedAfter.HasValue)
        {
            var after = filterParams.CreatedAfter.Value;
            spec = spec.And((u) => u.CreatedAt >= after);
        }

        if (filterParams.CreatedBefore.HasValue)
        {
            var before = filterParams.CreatedBefore.Value;
            spec = spec.And((u) => u.CreatedAt <= before);
        }

        if (filterParams.SortDir == "asc")
        {
            if (filterParams.SortField == "firstName")
            {
                AddOrderBy((u) => u!.FirstName!);
            }
            else if (filterParams.SortField == "lastName")
            {
                AddOrderBy((u) => u!.LastName!);
            }
            else if (filterParams.SortField == "createdAt")
            {
                AddOrderBy((u) => u.CreatedAt);
            }
        }
        else if(filterParams.SortDir == "desc")
        {
            if (filterParams.SortField == "firstName")
            {
                AddOrderByDescending((u) => u!.FirstName!);
            }
            else if (filterParams.SortField == "lastName")
            {
                AddOrderByDescending((u) => u!.LastName!);
            }
            else if (filterParams.SortField == "createdAt")
            {
                AddOrderByDescending((u) => u.CreatedAt);
            }
        }
        
        Criteria = spec;
        AddInclude((u) => u.Roles);
        var skip = (filterParams.Page - 1) * filterParams.Size;
        ApplyPaging(skip ?? default!, filterParams.Size ?? default!);
    }
}