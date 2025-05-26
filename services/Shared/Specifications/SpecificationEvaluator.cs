using Microsoft.EntityFrameworkCore;

namespace Shared.Specifications;

public class SpecificationEvaluator<TModel> where TModel : class
{
    public static IQueryable<TModel> GetQuery(IQueryable<TModel> inputQuery, ISpecification<TModel> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.Includes != null)
        {
            query = specification.Includes.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        if (specification.IncludeStrings != null)
        {
            query = specification.IncludeStrings.Aggregate(query,
                (current, includeExpression) => current.Include(includeExpression));
        }

        if (specification.SortExpressions?.Any() == true)
        {
            var first = true;
            foreach (var sort in specification.SortExpressions)
            {
                if (first)
                {
                    if (sort.Descending)
                    {
                        query = query.OrderByDescending(sort.Expression);
                    }
                    else
                    {
                        query = query.OrderBy(sort.Expression);
                    }
                    first = false;
                }
                else
                {
                    if (sort.Descending)
                    {
                        query = ((IOrderedQueryable<TModel>)query).ThenByDescending(sort.Expression);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<TModel>)query).ThenBy(sort.Expression);
                    }  
                }
            }
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip)
                        .Take(specification.Take);
        }

        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery<TModel>();
        }

        return query;
    }
}