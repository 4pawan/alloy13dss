# Dummy Tool File Inventory

This file lists the classes, Razor views, scripts, and small integration points used by the dummy form journey tool.

## New Dummy-Specific Classes

### Models

| Class | File | Purpose |
| --- | --- | --- |
| `DummyRootFolder` | `Models/Pages/DummyRootFolder.cs` | Non-rendered root folder for one reusable tool area. Holds the `SettingsPageLink` used by child dummy pages. |
| `DummySitePageData` | `Models/Pages/DummySitePageData.cs` | Base page class for dummy tool pages. Holds `FormToolKey` and `DirectFormContainerBlock`. |
| `DummyFormContainerBlock` | `Models/Forms/DummyFormContainerBlock.cs` | Custom Forms container that renders one child element at a time. |
| `DummyQuestionElementBlock` | `Models/Forms/DummyQuestionElementBlock.cs` | Custom question element with `Title`, source key, and visibility rules. |
| `DummyQuestionVisibilityRule` | `Models/Forms/DummyQuestionElementBlock.cs` | Rule row used by `DummyQuestionElementBlock.Rules`. |
| `DummyQuestionRuleOperator` | `Models/Forms/DummyQuestionElementBlock.cs` | Operators for question visibility rules. |
| `PropertyDummyQuestionVisibilityRuleList` | `Models/Forms/DummyQuestionElementBlock.cs` | Optimizely property-list backing type for question rules. |
| `DummyRecaptchaV3ElementBlock` | `Models/Forms/DummyRecaptchaV3ElementBlock.cs` | Hidden Forms element for reCAPTCHA v3 token support. |
| `DummyFormContainerViewModel` | `Models/ViewModels/DummyFormContainerViewModel.cs` | View model used by the dummy form container block renderer. |

### Business and Services

| Class or Interface | File | Purpose |
| --- | --- | --- |
| `DummyServiceCollectionExtensions` | `Business/Forms/DummyServiceCollectionExtensions.cs` | Registers dummy tool services through `services.AddDummyForms()`. |
| `IDummyToolSettingsResolver` | `Business/Forms/IDummyToolSettingsResolver.cs` | Resolves the nearest `DummyRootFolder` settings page. |
| `DummyToolSettingsResolver` | `Business/Forms/DummyToolSettingsResolver.cs` | Walks ancestors to find the nearest `DummyRootFolder.SettingsPageLink`. |
| `IDummyFormToolResolver` | `Business/Forms/IDummyFormToolResolver.cs` | Resolves the form container for a dummy tool page. |
| `DummyFormToolResolver` | `Business/Forms/DummyFormToolResolver.cs` | Uses `DirectFormContainerBlock` or `FormToolKey` plus root-folder settings to find a form container. |
| `IDummyFormJourneyService` | `Business/Forms/IDummyFormJourneyService.cs` | Builds the dummy journey view model and handles submissions. |
| `DummyFormJourneyService` | `Business/Forms/DummyFormJourneyService.cs` | Main journey orchestration: active element, previous/next, captcha, answers, branch reset. |
| `DummyFormJourneyPostModel` | `Business/Forms/DummyFormJourneyPostModel.cs` | POST payload for each journey step. |
| `DummyFormJourneyResult` | `Business/Forms/DummyFormJourneyResult.cs` | Submit result returned by the journey service. |
| `DummyFormJourneyState` | `Business/Forms/DummyFormJourneyState.cs` | DDS state: submission id, current element, visited elements. |
| `IDummyFormJourneyStateRepository` | `Business/Forms/IDummyFormJourneyStateRepository.cs` | Repository contract for journey state. |
| `DummyFormJourneyStateRepository` | `Business/Forms/DummyFormJourneyStateRepository.cs` | DDS-backed journey state repository. |
| `IDummyFormBranchEvaluator` | `Business/Forms/IDummyFormBranchEvaluator.cs` | Branch/visibility evaluator contract. |
| `DummyFormBranchEvaluator` | `Business/Forms/DummyFormBranchEvaluator.cs` | Calculates first and next renderable element based on submitted answers and rules. |
| `IDummyFormSubmissionAnswerStore` | `Business/Forms/IDummyFormSubmissionAnswerStore.cs` | Answer store contract backed by Optimizely Forms submissions. |
| `DummyFormSubmissionAnswerStore` | `Business/Forms/DummyFormSubmissionAnswerStore.cs` | Saves, reads, and clears answers from actual Forms submissions. |

### Controller

| Class | File | Purpose |
| --- | --- | --- |
| `DummyFormContainerBlockController` | `Controllers/DummyFormContainerBlockController.cs` | Renders `DummyFormContainerBlock` and handles subsequent journey submissions. |

## Tool Page and Settings Types

These page/block types are used by the dummy tool setup. Some are dummy-specific, and some are intentionally normal CMS types that the dummy tool uses.

| Class | File | Purpose |
| --- | --- | --- |
| `ContentPage` | `Models/Pages/ContentPage.cs` | Dummy layout page with a `ContentArea`; inherits `DummySitePageData`. |
| `LandingPage` | `Models/Pages/LandingPage.cs` | Uses dummy layout and can resolve a dummy form tool; inherits `DummySitePageData`. |
| `StandardPage` | `Models/Pages/StandardPage.cs` | Uses dummy layout and can resolve a dummy form tool; inherits `DummySitePageData`. |
| `SettingsPage` | `Models/Pages/SettingsPage.cs` | CMS-only settings page for reCAPTCHA, form tools, footer links, and GTM. |
| `FormToolSettingsBlock` | `Models/Blocks/FormToolSettingsBlock.cs` | One reusable tool definition inside `SettingsPage.FormTools`. |

## Razor Views and Partials

| `.cshtml` | Purpose |
| --- | --- |
| `Views/Shared/Blocks/DummyFormContainerBlock.cshtml` | Front-end renderer for the one-question-at-a-time form container. |
| `Views/Shared/Layouts/_DummyToolLayout.cshtml` | Self-contained dummy layout. Resolves root-folder settings for GTM/footer links. |
| `Views/Shared/Layouts/_DummyToolHeader.cshtml` | Dummy tool header; shows current page and active question title target. |
| `Views/Shared/Layouts/_DummyToolFooter.cshtml` | Dummy tool footer; renders `SettingsPage.FooterLinks`. |
| `Views/ContentPage/Index.cshtml` | View for `ContentPage`. |
| `Views/LandingPage/Index.cshtml` | Dummy layout view for `LandingPage`. |
| `Views/StandardPage/Index.cshtml` | Dummy layout view for `StandardPage`. |

## JavaScript

| File | Purpose |
| --- | --- |
| `wwwroot/js/maintool.js` | Copies the active question value into the journey POST model, updates header title, and requests reCAPTCHA token before submit. |

## Existing Integration Touchpoints

These existing files are touched only to wire the dummy tool into the Alloy project.

| File | Change |
| --- | --- |
| `Startup.cs` | Calls `services.AddDummyForms()` and maps the `dummy-form-journey` route. |
| `Business/PageViewContextFactory.cs` | Should remain generic Alloy context; dummy layout now resolves dummy settings itself. |
| `Models/Pages/SitePageData.cs` | Generic page base; dummy tool fields should live in `DummySitePageData`, not here. |

## CMS Setup Shape

```text
DummyRootFolder
  SettingsPageLink -> SettingsPage
  ContentPage / LandingPage / StandardPage
    ContentArea or page tool settings
      DummyFormContainerBlock
        ElementsArea
          DummyQuestionElementBlock
          DummyQuestionElementBlock
          DummyQuestionElementBlock
```

