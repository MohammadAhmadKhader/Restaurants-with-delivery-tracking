using FluentValidation;
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
}