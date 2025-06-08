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
}