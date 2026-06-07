# Dummy Form Container Architecture

This document explains the current dummy form journey architecture.

## Goal

`DummyFormContainerBlock` extends Optimizely Forms so a form can render one question at a time. Editors add normal/custom Forms elements to the form elements area. Each custom `DummyQuestionElementBlock` owns its own visibility rules.

The intended CMS shape is:

```text
DummyFormContainerBlock
  Forms Elements Area
    Q1 [DummyQuestionElementBlock]
    Q2 [DummyQuestionElementBlock]
    Q3 [DummyQuestionElementBlock]
```

Each `DummyQuestionElementBlock` can define rules that decide whether that question should be shown after previous responses are submitted.

## High-Level Flow

```text
First render
  -> render first DummyQuestionElementBlock from Forms Elements Area

Submit answer
  -> save submitted response
  -> scan the next questions in Forms Elements Area
  -> check each question's own visibility rules
  -> skip questions whose rules do not match
  -> render the next matching question
```

## Content Model

### DummyFormContainerBlock

File: `Models/Forms/DummyFormContainerBlock.cs`

This block extends `FormContainerBlock`.

It does not own journey rules directly anymore. The ordering comes from the standard Forms `ElementsArea`.

It does not own reCAPTCHA settings. Those live on `SettingsPage` so the same tool can be reused without copying integration keys onto every form container.

### DummyQuestionElementBlock

File: `Models/Forms/DummyQuestionElementBlock.cs`

This is the custom question element. It extends `TextboxElementBlock`.

It adds:

- `BranchingKey`: a stable key used by other questions to read this question's submitted value.
- `AlwaysRedirectToSourceKey`: optional source key of a question that should always be shown after this question is submitted.
- `Rules`: an `IList<DummyQuestionVisibilityRule>` configured directly on the question.

Example:

```text
Q1
  BranchingKey: customerType
  AlwaysRedirectToSourceKey:

Q2
  BranchingKey: companyName
  Rules:
    QuestionKey: customerType
    Operator: Equals
    Value: Business
```

Result:

```text
Q2 is shown only when Q1 response is Business.
```

### DummyQuestionVisibilityRule

File: `Models/Forms/DummyQuestionElementBlock.cs`

This is a simple rule object used inside `DummyQuestionElementBlock.Rules`.

It contains:

- `QuestionKey`: the `BranchingKey` of a previously submitted question.
- `Operator`: `Equals`, `NotEquals`, `Contains`, `NotContains`, `IsEmpty`, `IsNotEmpty`, `IsAnyOf`, or `IsNotAnyOf`.
- `Value`: comparison value.

All rules on a question must match for that question to be shown.

For `IsAnyOf` and `IsNotAnyOf`, enter multiple values in `Value` separated by comma, semicolon, or pipe.

The rule uses string keys instead of `ContentReference`, which avoids the `PropertyList<T>` serialization issue caused by serializing `ContentReference` internals.

## Runtime Services

### DummyFormContainerBlockController

File: `Controllers/DummyFormContainerBlockController.cs`

This is the MVC partial controller for the dummy form container.

It stays thin:

- `Index` builds the view model.
- `Submit` passes the post model to the journey service.
- Redirects back with `submissionId` in the query string.

### DummyFormJourneyService

File: `Business/Forms/DummyFormJourneyService.cs`

This service owns the journey flow.

On first render:

1. Resolve or create DDS journey state.
2. Ask `DummyFormBranchEvaluator.ResolveFirstElement` for the first question.
3. Render that question.

On submit:

1. Resolve the current form and journey state.
2. Handle Previous if requested.
3. Load the posted `SettingsPageLink` and validate reCAPTCHA v3 from those settings.
4. Save the submitted answer.
5. Ask `DummyFormBranchEvaluator.ResolveNextElement` for the next visible question.
6. Update DDS state.
7. Redirect back to the page.

### DummyFormBranchEvaluator

File: `Business/Forms/DummyFormBranchEvaluator.cs`

This evaluator now works from the form's standard `ElementsArea`.

It loads only `DummyQuestionElementBlock` items from `form.ElementsArea`.

`ResolveFirstElement`:

- Returns the first `DummyQuestionElementBlock` in the form elements area.

`ResolveNextElement`:

- Finds the current question in the ordered list.
- If the current question has `AlwaysRedirectToSourceKey`, jumps to the question with that `BranchingKey`.
- Scans following questions.
- For each next question, checks that question's own `Rules`.
- Returns the first question whose rules match.
- Returns `ContentReference.EmptyReference` when no more questions should be shown.

### DummyFormSubmissionAnswerStore

File: `Business/Forms/DummyFormSubmissionAnswerStore.cs`

This is currently an in-memory adapter.

When an answer is saved, it stores the value under:

- the element content id
- the question's `BranchingKey`, if configured

The `BranchingKey` entry is what visibility rules use.

Production note: this should be replaced with real Optimizely Forms submission storage so answers survive app restarts and can be resumed by submission id.

### DummyFormJourneyState

File: `Business/Forms/DummyFormJourneyState.cs`

Stored in DDS.

It tracks navigation state:

- `SubmissionId`
- `CurrentElement`
- `VisitedElements`
- `UpdatedUtc`

`FormContentLink` still exists in the current code as a defensive reference, but it can be removed if you want the form always resolved from page/settings.

## Views

### DummyFormContainerBlock.cshtml

File: `Views/Shared/Blocks/DummyFormContainerBlock.cshtml`

In edit mode:

- Renders the normal full Forms elements area so editors can edit all questions.

In view mode:

- Renders one active element only.
- Posts hidden journey fields.
- Posts the resolved `SettingsPageLink` so the next submission can validate reCAPTCHA server-side.
- Posts standard Forms hidden fields.
- Captures the active input value into `CurrentValue`.
- Executes reCAPTCHA v3 when configured.

The browser behavior is implemented in `wwwroot/js/maintool.js`.

## Settings Tool Architecture

### SettingsPage

File: `Models/Pages/SettingsPage.cs`

CMS-only settings page. It has no view.

It holds:

- `RecaptchaSiteKey`
- `RecaptchaSecretKey`
- `RecaptchaScoreThreshold`
- `FormTools`: content area of `FormToolSettingsBlock`

### FormToolSettingsBlock

File: `Models/Blocks/FormToolSettingsBlock.cs`

Defines a reusable form tool:

- `Key`
- `Name`
- `Description`
- `FormContainerBlock`

### FormToolResolver

File: `Business/Forms/FormToolResolver.cs`

Resolves the form tool for a page:

1. Use page `DirectFormContainerBlock` if set.
2. Otherwise load page `SettingsPageLink`.
3. Find `FormToolSettingsBlock` by `FormToolKey`.
4. Load that tool's form container.

## CMS Setup Example

1. Create a `SettingsPage`.
2. Add reCAPTCHA v3 keys and score threshold on that SettingsPage.
3. Create a `DummyFormContainerBlock`.
4. In the normal Forms elements area, add:

```text
Q1 DummyQuestionElementBlock
  BranchingKey: customerType

Q2 DummyQuestionElementBlock
  BranchingKey: companyName
  Rules:
    QuestionKey: customerType
    Operator: Equals
    Value: Business

Q3 DummyQuestionElementBlock
  BranchingKey: personalName
  Rules:
    QuestionKey: customerType
    Operator: Equals
    Value: Personal
```

Always-redirect example:

```text
Q4 DummyQuestionElementBlock
  BranchingKey: abc
  AlwaysRedirectToSourceKey: xyz

Q7 DummyQuestionElementBlock
  BranchingKey: xyz
```

Runtime:

```text
submit Q4 -> render Q7
```

Runtime:

```text
First render -> Q1

If Q1 = Business:
  submit Q1 -> Q2 rules pass -> render Q2

If Q1 = Personal:
  submit Q1 -> Q2 rules fail -> skip Q2 -> Q3 rules pass -> render Q3
```

## Important Files

- `Models/Forms/DummyFormContainerBlock.cs`
  - Custom form container.

- `Models/Forms/DummyQuestionElementBlock.cs`
  - Custom question element, optional always-redirect target, and visibility rule list.

- `Business/Forms/DummyFormBranchEvaluator.cs`
  - Calculates first and next visible question.

- `Business/Forms/DummyFormJourneyService.cs`
  - Orchestrates render and submit journey flow.

- `Business/Forms/DummyFormSubmissionAnswerStore.cs`
  - Stores submitted answers by element id and `BranchingKey`.

- `Business/Forms/DummyFormJourneyState.cs`
  - DDS navigation state.

- `Views/Shared/Blocks/DummyFormContainerBlock.cshtml`
  - Renders one active question at a time.

- `wwwroot/js/maintool.js`
  - Copies the active answer into `CurrentValue`.
  - Executes reCAPTCHA v3 before submit when configured.

## Current Limitations

- Answer storage is still in-memory and should be replaced with Optimizely Forms submissions.
- Element-level built-in Forms validation still needs full integration in the custom submit path.
- Branching is now modeled by hide/show rules on each next question, not by separate branch target blocks.
