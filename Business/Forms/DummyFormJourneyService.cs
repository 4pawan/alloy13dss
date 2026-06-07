using alloy13dss.Models.Forms;
using alloy13dss.Models.Pages;
using alloy13dss.Models.ViewModels;
using EPiServer.Forms.Core;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace alloy13dss.Business.Forms;

public class DummyFormJourneyService(
    IContentLoader contentLoader,
    IDummyFormJourneyStateRepository stateRepository,
    IDummyFormBranchEvaluator branchEvaluator,
    IDummyFormSubmissionAnswerStore answerStore,
    IRecaptchaV3Verifier recaptchaVerifier,
    IPageRouteHelper pageRouteHelper) : IDummyFormJourneyService
{
    public DummyFormContainerViewModel BuildViewModel(DummyFormContainerBlock form, Guid submissionId, ITempDataDictionary tempData)
    {
        var state = ResolveState(form, submissionId);
        var activeElement = ResolveActiveElement(state);
        var settingsPage = ResolveCurrentSettingsPage();

        return new DummyFormContainerViewModel
        {
            Form = form,
            FormContentLink = GetContentLink(form),
            SettingsPageLink = settingsPage?.ContentLink ?? ContentReference.EmptyReference,
            RecaptchaSiteKey = settingsPage?.RecaptchaSiteKey,
            ActiveElement = activeElement,
            ActiveElementLink = GetContentLink(activeElement),
            SubmissionId = state.SubmissionId,
            CanGoBack = state.VisitedElements.Count > 1,
            IsComplete = activeElement == null,
            Message = tempData["DummyFormJourneyMessage"] as string
        };
    }

    public async Task<DummyFormJourneyResult> SubmitAsync(DummyFormJourneyPostModel postModel, ITempDataDictionary tempData)
    {
        if (ContentReference.IsNullOrEmpty(postModel.FormContentLink) ||
            !contentLoader.TryGet(postModel.FormContentLink, out DummyFormContainerBlock form))
        {
            return new DummyFormJourneyResult { IsValid = false };
        }

        var state = ResolveState(form, postModel.SubmissionId == Guid.Empty ? Guid.NewGuid() : postModel.SubmissionId);
        var activeElement = ResolveActiveElement(state);

        if (activeElement == null)
        {
            return new DummyFormJourneyResult { IsValid = true };
        }

        if (string.Equals(postModel.Command, "previous", StringComparison.OrdinalIgnoreCase))
        {
            MovePrevious(state);
            stateRepository.Save(state);
            return new DummyFormJourneyResult { IsValid = true, SubmissionId = state.SubmissionId };
        }

        if (!await IsCaptchaValid(ResolveSettingsPage(postModel.SettingsPageLink), postModel.RecaptchaToken))
        {
            tempData["DummyFormJourneyMessage"] = "Captcha validation failed.";
            return new DummyFormJourneyResult { IsValid = true, SubmissionId = state.SubmissionId };
        }

        answerStore.SaveAnswer(state.SubmissionId, form, activeElement, postModel.CurrentValue);
        MoveNext(form, state);

        return new DummyFormJourneyResult { IsValid = true, SubmissionId = state.SubmissionId };
    }

    private DummyFormJourneyState ResolveState(DummyFormContainerBlock form, Guid submissionId)
    {
        submissionId = submissionId == Guid.Empty ? Guid.NewGuid() : submissionId;

        var state = stateRepository.Get(submissionId);
        if (state != null)
        {
            RestoreResumeElement(state);
            return state;
        }

        var firstElement = branchEvaluator.ResolveFirstElement(form);

        state = new DummyFormJourneyState
        {
            SubmissionId = submissionId,
            CurrentElement = firstElement
        };

        AddVisitedElement(state, firstElement);
        stateRepository.Save(state);

        return state;
    }

    private void MoveNext(DummyFormContainerBlock form, DummyFormJourneyState state)
    {
        var previousPath = state.VisitedElements.ToList();
        var nextElement = branchEvaluator.ResolveNextElement(form, state, answerStore.GetAnswers(state.SubmissionId, form));

        if (ContentReference.IsNullOrEmpty(nextElement))
        {
            state.CurrentElement = ContentReference.EmptyReference;
            stateRepository.Save(state);
            return;
        }

        ResetAbandonedBranchAnswers(form, state, previousPath, nextElement);
        AddVisitedElement(state, nextElement);
        state.CurrentElement = nextElement;
        stateRepository.Save(state);
    }

    private void RestoreResumeElement(DummyFormJourneyState state)
    {
        var resumeElement = ResolveResumeElement(state);
        if (!ContentReference.IsNullOrEmpty(resumeElement) && ContentReference.IsNullOrEmpty(state.CurrentElement))
        {
            state.CurrentElement = resumeElement;
            stateRepository.Save(state);
        }
    }

    private static ContentReference ResolveResumeElement(DummyFormJourneyState state)
    {
        return state.VisitedElements.LastOrDefault() ?? ContentReference.EmptyReference;
    }

    private ElementBlockBase ResolveActiveElement(DummyFormJourneyState state)
    {
        if (ContentReference.IsNullOrEmpty(state.CurrentElement))
        {
            return null;
        }

        return contentLoader.TryGet(state.CurrentElement, out ElementBlockBase element)
            ? element
            : null;
    }

    private SettingsPage ResolveCurrentSettingsPage()
    {
        return pageRouteHelper.Page is SitePageData sitePage
            ? ResolveSettingsPage(sitePage.SettingsPageLink)
            : null;
    }

    private SettingsPage ResolveSettingsPage(ContentReference settingsPageLink)
    {
        return !ContentReference.IsNullOrEmpty(settingsPageLink) &&
            contentLoader.TryGet(settingsPageLink, out SettingsPage settingsPage)
                ? settingsPage
                : null;
    }

    private async Task<bool> IsCaptchaValid(SettingsPage settingsPage, string token)
    {
        if (string.IsNullOrWhiteSpace(settingsPage?.RecaptchaSecretKey))
        {
            return true;
        }

        return await recaptchaVerifier.VerifyAsync(
            settingsPage.RecaptchaSecretKey,
            token,
            "dummy_form_step",
            settingsPage.RecaptchaScoreThreshold);
    }

    private void MovePrevious(DummyFormJourneyState state)
    {
        if (state.VisitedElements.Count <= 1)
        {
            return;
        }

        state.VisitedElements.RemoveAt(state.VisitedElements.Count - 1);
        state.CurrentElement = state.VisitedElements.Last();
    }

    private void ResetAbandonedBranchAnswers(
        DummyFormContainerBlock form,
        DummyFormJourneyState state,
        IReadOnlyList<ContentReference> previousPath,
        ContentReference nextElement)
    {
        var targetIndex = previousPath.ToList().FindIndex(x => SameContent(x, nextElement));
        if (targetIndex < 0)
        {
            return;
        }

        var abandonedElements = previousPath.Skip(targetIndex + 1).ToList();
        answerStore.ClearAnswers(state.SubmissionId, form, abandonedElements);
        stateRepository.ResetBranch(state, nextElement);
    }

    private void AddVisitedElement(DummyFormJourneyState state, ContentReference elementLink)
    {
        if (ContentReference.IsNullOrEmpty(elementLink) || state.VisitedElements.Any(x => SameContent(x, elementLink)))
        {
            return;
        }

        state.VisitedElements.Add(elementLink);
    }

    private static bool SameContent(ContentReference left, ContentReference right)
    {
        return !ContentReference.IsNullOrEmpty(left)
            && !ContentReference.IsNullOrEmpty(right)
            && left.ID == right.ID
            && left.ProviderName == right.ProviderName;
    }

    private static ContentReference GetContentLink(IContentData contentData)
    {
        return contentData is IContent content ? content.ContentLink : ContentReference.EmptyReference;
    }
}
