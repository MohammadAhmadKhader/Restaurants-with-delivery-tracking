using System.Linq.Expressions;

namespace Shared.Specifications;
public interface ISpecification<TModel>
{
    bool IsSplitQuery { get; set; }
    Expression<Func<TModel, bool>> Criteria { get; set; }
    IList<Expression<Func<TModel, object>>> Includes { get; set; }
    IList<string> IncludeStrings { get; set; }
    IList<SortExpression<TModel>> SortExpressions { get; set; }
    int Take { get; set; }
    int Skip { get; set; }
    bool IsPagingEnabled { get; set; }
}