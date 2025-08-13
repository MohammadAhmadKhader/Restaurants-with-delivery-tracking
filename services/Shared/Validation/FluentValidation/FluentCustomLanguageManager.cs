using FluentValidation.Resources;

namespace Shared.Validation.FluentValidation;
// TODO: Fix localization on custom messages
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

        AddTranslation("en", CustomValidationErrorCodes.NotEmptyValidator_String, ValidationMessageTemplates.REQUIRED_FIELD);

        AddTranslation("en", "GreaterThanValidator", ValidationMessageTemplates.GREATER_THAN_VALIDATOR);

        AddTranslation("en", "GreaterThanOrEqualValidator", ValidationMessageTemplates.GREATER_THAN_OR_EQUAL_VALIDATOR);

        AddTranslation("en", "LessThanValidator", ValidationMessageTemplates.LESS_THAN_VALIDATOR);

        AddTranslation("en", "LessThanOrEqualValidator", ValidationMessageTemplates.LESS_THAN_OR_EQUAL_VALIDATOR);

        AddTranslation("en", CustomValidationErrorCodes.PhoneNumberValidator, ValidationMessageTemplates.PHONE_NUMBER_VALIDATOR);

        AddTranslation("en", CustomValidationErrorCodes.FileSizeValidator, ValidationMessageTemplates.FILE_SIZE_VALIDATOR);

        AddTranslation("en", CustomValidationErrorCodes.FileNotEmptyValidator, ValidationMessageTemplates.FILE_NOT_EMPTY_VALIDATOR);

        AddTranslation("en", CustomValidationErrorCodes.FileExtensionValidator, ValidationMessageTemplates.FILE_EXTENSION_VALIDATOR);
        
        AddTranslation("en", CustomValidationErrorCodes.IdsListDistinctValidator, ValidationMessageTemplates.IDS_LIST_DISTINCT_VALIDATOR);
    }
}