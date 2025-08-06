using FluentValidation.Resources;
using Shared.Constants;

namespace Shared.Common;

public class FluentCustomLanguageManager : LanguageManager
{
    public FluentCustomLanguageManager()
    {
        AddTranslation("en", "LengthValidator", ValidationMessageTemplates.LENGTH_VALIDATOR);

        AddTranslation("en", "NotNullValidator", ValidationMessageTemplates.REQUIRED_FIELD);

        AddTranslation("en", "InclusiveBetweenValidator", ValidationMessageTemplates.INCLUSIVE_VALIDATOR);

        AddTranslation("en", "MaximumLengthValidator", ValidationMessageTemplates.MAXIMUM_LENGTH_VALIDATOR);

        AddTranslation("en", "MinimumLengthValidator", ValidationMessageTemplates.MINIMUM_LENGTH_VALIDATOR);

        AddTranslation("en", "EmailValidator", ValidationMessageTemplates.EMAIL_VALIDATOR);

        AddTranslation("en", "NotEmptyValidator_String", ValidationMessageTemplates.REQUIRED_FIELD);

        AddTranslation("en", "GreaterThanValidator", ValidationMessageTemplates.GREATER_THAN_VALIDATOR);

        AddTranslation("en", "GreaterThanOrEqualValidator", ValidationMessageTemplates.GREATER_THAN_OR_EQUAL_VALIDATOR);

        AddTranslation("en", "LessThanValidator", ValidationMessageTemplates.LESS_THAN_VALIDATOR);
        
        AddTranslation("en", "LessThanOrEqualValidator", ValidationMessageTemplates.LESS_THAN_OR_EQUAL_VALIDATOR);
    }
}