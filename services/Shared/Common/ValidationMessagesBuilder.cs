using Shared.Constants;

namespace Shared.Common;

public static class ValidationMessagesBuilder
{
    public static string LengthBetween(string propertyName, int minLength, int maxLength) =>
        ValidationMessageTemplates.LENGTH_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{MinLength}", minLength.ToString())
            .Replace("{MaxLength}", maxLength.ToString());

    public static string MaxLength(string propertyName, int maxLength) =>
        ValidationMessageTemplates.MAXIMUM_LENGTH_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{MaxLength}", maxLength.ToString());

    public static string MinLength(string propertyName, int minLength) =>
        ValidationMessageTemplates.MINIMUM_LENGTH_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{MinLength}", minLength.ToString());

    public static string AtLeastOneRequired(params string[] fieldNames) =>
        string.Format(ValidationMessageTemplates.AT_LEAST_ONE_REQUIRED,
            string.Join(" or ", fieldNames.Select(f => $"'{f}'")));

    public static string Required(string fieldName) =>
        ValidationMessageTemplates.REQUIRED_FIELD
            .Replace("{PropertyName}", fieldName);

    public static string InvalidEmail() => ValidationMessageTemplates.EMAIL_VALIDATOR;

    public static string InclusiveBetween(string propertyName, int from, int to) =>
    ValidationMessageTemplates.INCLUSIVE_VALIDATOR
        .Replace("{PropertyName}", propertyName)
        .Replace("{From}", from.ToString())
        .Replace("{To}", to.ToString());

    public static string GreaterThan(string propertyName, object comparisonValue) =>
        ValidationMessageTemplates.GREATER_THAN_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{ComparisonValue}", comparisonValue.ToString());

    public static string GreaterThanOrEqual(string propertyName, object comparisonValue) =>
        ValidationMessageTemplates.GREATER_THAN_OR_EQUAL_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{ComparisonValue}", comparisonValue.ToString());

    public static string LessThan(string propertyName, object comparisonValue) =>
        ValidationMessageTemplates.LESS_THAN_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{ComparisonValue}", comparisonValue.ToString());

    public static string LessThanOrEqual(string propertyName, object comparisonValue) =>
        ValidationMessageTemplates.LESS_THAN_OR_EQUAL_VALIDATOR
            .Replace("{PropertyName}", propertyName)
            .Replace("{ComparisonValue}", comparisonValue.ToString());

    public static string InvalidId() => ValidationMessageTemplates.INVALID_ID;
}