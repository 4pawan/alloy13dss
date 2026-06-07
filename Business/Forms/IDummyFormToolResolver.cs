using alloy13dss.Models.Forms;
using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public interface IDummyFormToolResolver
{
    DummyFormContainerBlock Resolve(DummySitePageData page);
}
