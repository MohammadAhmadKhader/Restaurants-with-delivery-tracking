using System.Linq.Expressions;

namespace Shared.Specifications;

public static class PredicateBuilder
{
    public static Expression<Func<TModel, bool>> True<TModel>() => f => true;
    public static Expression<Func<TModel, bool>> And<TModel>(this Expression<Func<TModel, bool>> expr1, Expression<Func<TModel, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<TModel, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<TModel, bool>> Or<TModel>(this Expression<Func<TModel, bool>> expr1, Expression<Func<TModel, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<TModel, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }
}