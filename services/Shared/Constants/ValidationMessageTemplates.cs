namespace Shared.Constants;
public static class ValidationMessageTemplates
{
    public const string LENGTH_VALIDATOR = "{PropertyName} must be between {MinLength} and {MaxLength} characters.";
    public const string MAXIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at most {MaxLength} characters.";
    public const string MINIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at least {MinLength} characters.";
    public const string EMAIL_VALIDATOR = "Invalid email.";
    public const string AT_LEAST_ONE_REQUIRED = "At least one of {0} must be provided.";
    public const string REQUIRED_FIELD = "{PropertyName} is required.";
}