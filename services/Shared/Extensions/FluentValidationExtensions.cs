using FluentValidation;
using Shared.Constants;
using Shared.Utils;

namespace Shared.Extensions;

public static class FluentValidationExtensions
{
    public static void ApplyDefaultConfigurations(this ValidatorConfiguration global)
    {
        global.LanguageManager = FluentValidationUtils.AppLanguageManager;
        global.DisplayNameResolver = FluentValidationUtils.AppDisplayNameResolver;
    }

    public static IRuleBuilderOptions<T, string?> NotEmptyString<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode("NotEmptyValidator_String");
    }

     public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveUniqueValues<T, TItem, TProperty>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, TProperty> propertySelector)
    {
        return ruleBuilder.Must(items =>
        {
            if (items == null)
            {
                return true;
            }

            var itemList = items.ToList();
            var values = itemList.Select(propertySelector).ToList();
            return values.Count == values.Distinct().Count();
        });
    }
}