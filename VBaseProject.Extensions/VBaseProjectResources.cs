using Microsoft.Extensions.Localization;

namespace VBaseProject.Resources
{
    public interface ISharedResource
    {
    }

    public class VBaseProjectResources : ISharedResource
    {
        private readonly IStringLocalizer _localizer;

        public VBaseProjectResources(IStringLocalizer<VBaseProjectResources> localizer)
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
}
