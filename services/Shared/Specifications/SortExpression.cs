using System.Linq.Expressions;

namespace Shared.Specifications;
public record SortExpression<TModel>(Expression<Func<TModel, object>> Expression, bool Descending);