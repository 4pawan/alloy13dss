using alloy13dss.Business.Forms;
using alloy13dss.Models.Forms;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace alloy13dss.Controllers;

[TemplateDescriptor(
    ModelType = typeof(DummyFormContainerBlock),
    Inherited = true,
    TemplateTypeCategory = TemplateTypeCategories.MvcPartial,
    AvailableWithoutTag = true)]
public class DummyFormContainerBlockController(
    IDummyFormJourneyService journeyService) : ActionControllerBase
{
    public IActionResult Index(DummyFormContainerBlock currentBlock)
    {
        return View(
            "~/Views/Shared/Blocks/DummyFormContainerBlock.cshtml",
            journeyService.BuildViewModel(currentBlock, ResolveSubmissionId(), TempData));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(DummyFormJourneyPostModel postModel)
    {
        var result = await journeyService.SubmitAsync(postModel, TempData);
        if (!result.IsValid)
        {
            return BadRequest();
        }

        return RedirectBack(postModel.ReturnUrl, result.SubmissionId);
    }

    private Guid ResolveSubmissionId()
    {
        var rawSubmissionId = HttpContext.Request.Query["submissionId"].FirstOrDefault()
            ?? HttpContext.Request.Form["SubmissionId"].FirstOrDefault();

        return Guid.TryParse(rawSubmissionId, out var submissionId) ? submissionId : Guid.NewGuid();
    }

    private RedirectResult RedirectBack(string returnUrl, Guid? submissionId)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            return Redirect("~/");
        }

        if (!submissionId.HasValue)
        {
            return Redirect(returnUrl);
        }

        var separator = returnUrl.Contains('?') ? "&" : "?";
        return Redirect($"{returnUrl}{separator}submissionId={submissionId.Value}");
    }
}
