using System.Linq.Expressions;

namespace Shared.Specifications;
public abstract class BaseSpecification<TModel> : ISpecification<TModel>
{
    protected BaseSpecification()
    {
    }

    public Expression<Func<TModel, bool>> Criteria { get; set; }
    public IList<Expression<Func<TModel, object>>> Includes { get; set; }
    public IList<string> IncludeStrings { get; set; }
    public IList<SortExpression<TModel>> SortExpressions { get; set; }
    public int Take { get; set; }
    public int Skip { get; set; }
    public bool IsPagingEnabled { get; set; }
    public bool IsSplitQuery { get; set; }

    protected virtual void AddInclude(Expression<Func<TModel, object>> includeExpression)
    {
        (Includes ??= []).Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        (IncludeStrings ??= []).Add(includeString);
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected virtual void AddOrderBy(Expression<Func<TModel, object>> orderByExpression)
            => (SortExpressions ??= []).Add(new(orderByExpression, false));

    protected virtual void AddOrderByDescending(Expression<Func<TModel, object>> orderByDescExpression)
        => (SortExpressions ??= []).Add(new(orderByDescExpression, true));
}