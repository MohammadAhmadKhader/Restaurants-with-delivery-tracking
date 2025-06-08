using FluentValidation.Resources;
using Shared.Constants;

namespace Shared.Common;

public class FluentCustomLanguageManager : LanguageManager
{
    public FluentCustomLanguageManager()
    {
        AddTranslation("en", "LengthValidator", ValidationMessageTemplates.LENGTH_VALIDATOR);
        AddTranslation("en-US", "LengthValidator", ValidationMessageTemplates.LENGTH_VALIDATOR);
        AddTranslation("en-GB", "LengthValidator", ValidationMessageTemplates.LENGTH_VALIDATOR);

        AddTranslation("en", "MaximumLengthValidator", ValidationMessageTemplates.MAXIMUM_LENGTH_VALIDATOR);
        AddTranslation("en-US", "MaximumLengthValidator", ValidationMessageTemplates.MAXIMUM_LENGTH_VALIDATOR);
        AddTranslation("en-GB", "MaximumLengthValidator", ValidationMessageTemplates.MAXIMUM_LENGTH_VALIDATOR);

        AddTranslation("en", "MinimumLengthValidator", ValidationMessageTemplates.MINIMUM_LENGTH_VALIDATOR);
        AddTranslation("en-US", "MinimumLengthValidator", ValidationMessageTemplates.MINIMUM_LENGTH_VALIDATOR);
        AddTranslation("en-GB", "MinimumLengthValidator", ValidationMessageTemplates.MINIMUM_LENGTH_VALIDATOR);

        AddTranslation("en", "EmailValidator", ValidationMessageTemplates.EMAIL_VALIDATOR);
        AddTranslation("en-US", "EmailValidator", ValidationMessageTemplates.EMAIL_VALIDATOR);
        AddTranslation("en-GB", "EmailValidator", ValidationMessageTemplates.EMAIL_VALIDATOR);
    }
}