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
    }
}