using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Shared.Extensions;

public static class LocalizationExtensions
{
    public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("ar"),
            new CultureInfo("es"),
            new CultureInfo("fr"),
            new CultureInfo("ru"),
            new CultureInfo("it"),
            new CultureInfo("de"), // German
        };

        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedUICultures = supportedCultures
        };

        app.UseRequestLocalization(localizationOptions);

        return app;
    }
}