using Microsoft.Extensions.Localization;

namespace Orion.Croscutting.Resources;

public class OrionResources : ISharedResource
{
    private readonly IStringLocalizer _localizer;

    public OrionResources(IStringLocalizer<OrionResources> localizer)
    {
        _localizer = localizer;
    }

    public string this[string index]
    {
        get
        {
            return _localizer[index];
        }
    }
}
