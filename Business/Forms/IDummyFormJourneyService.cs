using alloy13dss.Models.Forms;
using alloy13dss.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace alloy13dss.Business.Forms;

public interface IDummyFormJourneyService
{
    DummyFormContainerViewModel BuildViewModel(DummyFormContainerBlock form, Guid submissionId, ITempDataDictionary tempData);

    Task<DummyFormJourneyResult> SubmitAsync(DummyFormJourneyPostModel postModel, ITempDataDictionary tempData);
}
