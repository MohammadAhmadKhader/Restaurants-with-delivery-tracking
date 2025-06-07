using FluentValidation.Resources;

namespace Shared.Common;

public class FluentCustomLanguageManager : LanguageManager
{
    public FluentCustomLanguageManager()
    {
        AddTranslation("en", "LengthValidator", "{PropertyName} must be between {MinLength} and {MaxLength} characters.");
        AddTranslation("en-US", "LengthValidator", "{PropertyName} must be between {MinLength} and {MaxLength} characters.");
        AddTranslation("en-GB", "LengthValidator", "{PropertyName} must be between {MinLength} and {MaxLength} characters.");

        AddTranslation("en", "MaximumLengthValidator", "{PropertyName} must be at most {MaxLength} characters.");
        AddTranslation("en-US", "MaximumLengthValidator", "{PropertyName} must be at most {MaxLength} characters.");
        AddTranslation("en-GB", "MaximumLengthValidator", "{PropertyName} must be at most {MaxLength} characters.");

        AddTranslation("en", "MinimumLengthValidator", "{PropertyName} must be at least {MinLength} characters.");
        AddTranslation("en-US", "MinimumLengthValidator", "{PropertyName} must be at least {MinLength} characters.");
        AddTranslation("en-GB", "MinimumLengthValidator", "{PropertyName} must be at least {MinLength} characters.");

        AddTranslation("en", "EmailValidator", "Invalid email.");
        AddTranslation("en-US", "EmailValidator", "Invalid email.");
        AddTranslation("en-GB", "EmailValidator", "Invalid email.");
    }
}