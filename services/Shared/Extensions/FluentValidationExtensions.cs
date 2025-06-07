using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Validators;
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