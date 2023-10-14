using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Orion.Api.Configuration;

public static class GlobalizationConfiguration
{
    public static void ConfigureGlobalization(this IApplicationBuilder app)
    {
        var defaultCultureInfo = new CultureInfo("pt-BR")
        {
            NumberFormat =
            {
                CurrencySymbol = "R$"
            }
        };

        var supportedCultures = new List<CultureInfo>
        {
            defaultCultureInfo,
            new CultureInfo("en-US")
        };

        var globalizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures,
        };

        app.UseRequestLocalization(globalizationOptions);

        CultureInfo.DefaultThreadCurrentCulture = defaultCultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = defaultCultureInfo;
    }
}
