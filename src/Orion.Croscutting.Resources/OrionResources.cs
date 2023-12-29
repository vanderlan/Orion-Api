using Microsoft.Extensions.Localization;

namespace Orion.Croscutting.Resources;

public class OrionResources(IStringLocalizer<OrionResources> localizer) : ISharedResource
{
    private readonly IStringLocalizer _localizer = localizer;

    public string this[string index] => _localizer[index];
}
