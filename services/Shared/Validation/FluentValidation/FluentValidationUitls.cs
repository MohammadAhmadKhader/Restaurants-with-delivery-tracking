using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Results;
using Shared.Utils;

namespace Shared.Validation.FluentValidation;

public static class FluentValidationUtils
{
    public static Dictionary<string, Dictionary<string, string>> FieldTranslations { get; } = [];
    public static List<(string field, string message)> HandleValidationErrors(ValidationResult result)
    {
        return result.Errors
            .Select(e => (field: GeneralUtils.PascalToCamel(e.PropertyName), message: e.ErrorMessage))
            .ToList();
    }

    public static readonly Func<Type, MemberInfo, LambdaExpression, string> AppDisplayNameResolver = (type, memberInfo, expression) =>
    {
        if (memberInfo != null)
        {
            var originalName = memberInfo.Name;
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (FieldTranslations.TryGetValue(culture, out var translations) &&
                   translations.TryGetValue(originalName, out var translated))
            {
                return translated;
            }

            return originalName;
        }

        return ValidatorOptions.Global.DisplayNameResolver(type, memberInfo, expression);
    };

    public static readonly ILanguageManager AppLanguageManager = new FluentCustomLanguageManager();
}