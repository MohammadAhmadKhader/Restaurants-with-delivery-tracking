namespace Shared.Validation.FluentValidation;

public static class ValidationMessageTemplates
{
    public const string LENGTH_VALIDATOR = "{PropertyName} must be between {MinLength} and {MaxLength} characters.";
    public const string INCLUSIVE_VALIDATOR = "{PropertyName} must be between {From} and {To}.";
    public const string MAXIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at most {MaxLength} characters.";
    public const string MINIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at least {MinLength} characters.";
    public const string EMAIL_VALIDATOR = "Invalid email.";
    public const string AT_LEAST_ONE_REQUIRED = "At least one of {0} must be provided.";
    public const string REQUIRED_FIELD = "{PropertyName} is required.";

    public const string GREATER_THAN_VALIDATOR = "'{PropertyName}' must be greater than {ComparisonValue}.";
    public const string GREATER_THAN_OR_EQUAL_VALIDATOR = "'{PropertyName}' must be greater than or equal to {ComparisonValue}.";
    public const string LESS_THAN_VALIDATOR = "'{PropertyName}' must be less than {ComparisonValue}.";
    public const string LESS_THAN_OR_EQUAL_VALIDATOR = "'{PropertyName}' must be less than or equal to {ComparisonValue}.";
    public const string INVALID_ID = "Invalid id.";

    public const string FILE_EXTENSION_VALIDATOR = "Only {0} files are allowed.";
    public const string FILE_SIZE_VALIDATOR = "File size must be {0}MB or less.";
    public const string FILE_NOT_EMPTY_VALIDATOR = "File cannot be empty.";

    public const string PHONE_NUMBER_VALIDATOR = "Invalid phone number format.";

    public const string IDS_LIST_DISTINCT_VALIDATOR = "Duplicate '{PropertyName}' are not allowed.";
}