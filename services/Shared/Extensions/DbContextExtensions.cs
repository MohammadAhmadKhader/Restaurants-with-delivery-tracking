using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Utils;

namespace Shared.Extensions;

public static class DbContextExtensions
{
    public static void AddGuidNotEmptyConstraint<TEntity>(
        this EntityTypeBuilder<TEntity> entityBuilder,
        string propertyName,
        string? modelName = null) where TEntity : class
    {
        modelName ??= typeof(TEntity).Name;
        var constraintName = DbContextUtils.GenerateConstraintName(modelName, propertyName, "NotEmpty");
        var constraintSql = DbContextUtils.GetGuidNotEmptyConstraintPgSql(propertyName);

        entityBuilder.ToTable(t => t.HasCheckConstraint(constraintName, constraintSql));
    }
}