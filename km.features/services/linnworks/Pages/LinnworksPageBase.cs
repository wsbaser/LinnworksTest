using Natu.Utils.Extensions;
using selenium.core.Framework.Page;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public abstract class LinnworksPageBase : SelfMatchingPageBase, IHasWebElements
    {
        public virtual T GetElement<T>(string elementName) => (T)Components[elementName.AddSpaces()];
    }

    public interface IHasWebElements
    {
        T GetElement<T>(string elementName);
    }
}
