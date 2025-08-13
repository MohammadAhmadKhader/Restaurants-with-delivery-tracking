using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Shared.Validation.FluentValidation;

public static class FluentValidationExtensions
{
    private static readonly string[] defaultAllowedExtensions = [".jpg", ".jpeg", ".png"];
    public static void ApplyDefaultConfigurations(this ValidatorConfiguration global)
    {
        global.LanguageManager = FluentValidationUtils.AppLanguageManager;
        global.DisplayNameResolver = FluentValidationUtils.AppDisplayNameResolver;
    }

    public static IRuleBuilderOptions<T, string?> NotEmptyString<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode(CustomValidationErrorCodes.NotEmptyValidator_String);
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
        })
        .WithErrorCode(CustomValidationErrorCodes.IdsListDistinctValidator)
        .WithName("Items");
    }

    public static IRuleBuilderOptions<T, IFormFile?> ValidImageFile<T>(
        this IRuleBuilder<T, IFormFile?> ruleBuilder,
        long maxFileSizeInBytes = 5 * 1024 * 1024, // 5MB default
        string[]? allowedExtensions = null)
    {
        allowedExtensions ??= defaultAllowedExtensions;

        return ruleBuilder
            .Must(file => file == null || file.Length > 0)
                .WithErrorCode(CustomValidationErrorCodes.FileNotEmptyValidator)
                .WithMessage(ValidationMessagesBuilder.FileNotEmpty())
            .Must(file => file == null || file.Length <= maxFileSizeInBytes)
                .WithErrorCode(CustomValidationErrorCodes.FileSizeValidator)
                .WithMessage(ValidationMessagesBuilder.FileSize(maxFileSizeInBytes))
            .Must(file => file == null || allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithErrorCode(CustomValidationErrorCodes.FileExtensionValidator);
    }

    public static IRuleBuilderOptions<T, string?> ValidPhoneNumber<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => new PhoneAttribute().IsValid(x))
            .WithErrorCode(CustomValidationErrorCodes.PhoneNumberValidator);
    }
}

