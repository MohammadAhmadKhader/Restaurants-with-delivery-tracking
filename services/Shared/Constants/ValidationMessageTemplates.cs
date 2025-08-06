namespace Shared.Constants;

public static class ValidationMessageTemplates
{
    public const string LENGTH_VALIDATOR = "{PropertyName} must be between {MinLength} and {MaxLength} characters.";
    public const string INCLUSIVE_VALIDATOR = "{PropertyName} must be between {From} and {To}.";
    public const string MAXIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at most {MaxLength} characters.";
    public const string MINIMUM_LENGTH_VALIDATOR = "{PropertyName} must be at least {MinLength} characters.";
    public const string EMAIL_VALIDATOR = "Invalid email.";
    public const string AT_LEAST_ONE_REQUIRED = "At least one of {0} must be provided.";
    public const string REQUIRED_FIELD = "{PropertyName} is required.";
    
    public const string GREATER_THAN_VALIDATOR = "'{PropertyName}' must be greater than aa{ComparisonValue}.";
    public const string GREATER_THAN_OR_EQUAL_VALIDATOR = "'{PropertyName}' must be greater than or equal to aa{ComparisonValue}.";
    public const string LESS_THAN_VALIDATOR = "'{PropertyName}' must be less than aa{ComparisonValue}.";
    public const string LESS_THAN_OR_EQUAL_VALIDATOR = "'{PropertyName}' must be less than or equal to aa{ComparisonValue}.";
    public const string INVALID_ID = "Invalid id.";
}