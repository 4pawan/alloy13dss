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
        if (!ModelState.IsValid)
        {
            TempData["DummyFormJourneyMessage"] = ResolveModelStateMessage();
            return RedirectBack(postModel.ReturnUrl, postModel.SubmissionId == Guid.Empty ? null : postModel.SubmissionId);
        }

        var result = await journeyService.SubmitAsync(postModel, TempData);
        if (!result.IsValid)
        {
            TempData["DummyFormJourneyMessage"] = "Unable to continue this form. Please refresh and try again.";
            return RedirectBack(postModel.ReturnUrl, postModel.SubmissionId == Guid.Empty ? null : postModel.SubmissionId);
        }

        return RedirectBack(postModel.ReturnUrl, result.SubmissionId);
    }

    private string ResolveModelStateMessage()
    {
        var firstError = ModelState
            .Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));

        if (!string.IsNullOrWhiteSpace(firstError))
        {
            return firstError;
        }

        if (ModelState.Values.SelectMany(value => value.Errors).Any(error => error.Exception != null))
        {
            return "Some submitted values could not be read. Please check the current step and try again.";
        }

        return "Please check the current step and try again.";
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
