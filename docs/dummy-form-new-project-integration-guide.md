# Dummy Form New Project Integration Guide

Use this guide to integrate the dummy form journey into a new Optimizely project gradually. Do not copy everything at once. Add one layer, build, test, then move to the next layer.

## Minimum Architecture

```text
Page
  -> resolves DummyFormContainerBlock
    -> renders one DummyQuestionElementBlock at a time
      -> saves answer into Optimizely Forms submission
      -> stores journey navigation in DDS
      -> calculates next question from rules
```

## Step 1: Install Packages

Add Optimizely Forms packages:

```xml
<PackageReference Include="EPiServer.Forms" Version="..." />
<PackageReference Include="EPiServer.Forms.Core" Version="..." />
<PackageReference Include="EPiServer.Forms.UI" Version="..." />
```

Enable Forms:

```csharp
services
    .AddCms()
    .AddForms();
```

Verify:

- CMS starts.
- Forms UI is available.

## Step 2: Add The Form Container

Create:

```csharp
public class DummyFormContainerBlock : FormContainerBlock
{
}
```

Add a simple `.cshtml` view that renders the full form normally first. Do not add one-question journey logic yet.

Verify:

- You can create the block.
- You can add normal Forms elements.
- It renders on a page.

## Step 3: Add The Custom Question Element

Create:

```csharp
public class DummyQuestionElementBlock : TextboxElementBlock
{
    public virtual string BranchingKey { get; set; }

    [BackingType(typeof(PropertyDummyQuestionVisibilityRuleList))]
    public virtual IList<DummyQuestionVisibilityRule> Rules { get; set; }
}
```

Add:

```csharp
public class DummyQuestionVisibilityRule
{
    public virtual string QuestionKey { get; set; }
    public virtual DummyQuestionRuleOperator Operator { get; set; }
    public virtual string Value { get; set; }
}
```

Start with only:

```csharp
public enum DummyQuestionRuleOperator
{
    Equals,
    NotEquals,
    IsEmpty,
    IsNotEmpty
}
```

Add more operators later.

Verify:

- Editors can add `DummyQuestionElementBlock`.
- Editors can set `BranchingKey`.
- Editors can add rules.

## Step 4: Add Minimal Journey State In DDS

Keep DDS state small:

```csharp
public class DummyFormJourneyState : IDynamicData
{
    public Identity Id { get; set; }
    public Guid SubmissionId { get; set; }
    public ContentReference CurrentElement { get; set; }
    public List<ContentReference> VisitedElements { get; set; } = [];
    public DateTime UpdatedUtc { get; set; }
}
```

Do not store answers in DDS. Do not store form link in DDS.

Verify:

- You can create, load, and save state by `SubmissionId`.
- Refresh keeps current question.

## Step 5: Render One Element At A Time

Update the container view/controller/service:

```text
First render:
  Load state by submissionId
  If no state:
    pick first question from ElementsArea
    save state
  Render only CurrentElement
```

The view should post:

```text
FormContentLink
SubmissionId
CurrentElementLink
CurrentValue
Command
ReturnUrl
```

Verify:

- Only Q1 renders.
- Submitting Q1 redirects back.
- State keeps the same submission id.

## Step 6: Save Answers To Optimizely Forms Submission

Add `IDummyFormSubmissionAnswerStore`.

Implementation should use:

- `IFormDataRepository` to read existing submission.
- `DataSubmissionService.StoreSubmissionData(...)` to save or update.
- `FormIdentity` from `form.Form.FormGuid` and `form.Form.Language`.

Key idea:

- Store answers by Forms element field name.
- When reading answers, map each question's `BranchingKey` to its stored field value.

Verify:

- Submit Q1.
- Check Forms submission storage.
- Resume with same `submissionId`.
- Answer can be read back by `BranchingKey`.

## Step 7: Add Next-Question Evaluation

Create `DummyFormBranchEvaluator`.

Logic:

```text
Find current question index
Scan next questions in ElementsArea
For each candidate:
  evaluate candidate.Rules against submitted answers
  first matching question is next
If none:
  journey complete
```

Do not add branch blocks. Do not add condition blocks. Keep rules directly on questions.

Verify:

```text
Q1 customerType = Business
Q2 rule customerType Equals Business
Q3 rule customerType Equals Personal
```

Expected:

- Business -> Q2
- Personal -> Q3

## Step 8: Add Back Button

Use `VisitedElements`.

Previous:

```text
Remove current from visited list
Set CurrentElement = previous visited element
```

Verify:

- Q1 -> Q2 -> Previous returns Q1.
- Answer remains in Forms submission.

## Step 9: Branch Reset

When user changes an earlier answer and next path changes:

```text
Find nextElement in previous visited path
Clear abandoned question answers from Forms submission
Trim VisitedElements
Set CurrentElement = nextElement
```

Verify:

- User goes Business path.
- User goes back to Q1.
- User changes answer to Personal.
- Old Business-only question answer is cleared.

## Step 10: Resume Summary

No special property is needed.

Example setup:

```text
Q1
Q2
Q3
Q4
Q5
Summary
```

Once Summary is visited, it is the last item in `VisitedElements`.

Resume behavior:

```text
If CurrentElement is empty:
  restore last visited element
```

So resume returns to Summary.

## Step 11: Add SettingsPage

Add CMS-only settings page:

```csharp
public class SettingsPage : SitePageData
{
    public virtual ContentArea FormTools { get; set; }

    public virtual string RecaptchaSiteKey { get; set; }

    [ScaffoldColumn(false)]
    public virtual string RecaptchaSecretKey { get; set; }

    public virtual double RecaptchaScoreThreshold { get; set; }
}
```

Do not create a view for `SettingsPage`.

Verify:

- Page can reference `SettingsPage`.
- `SettingsPage` can hold reusable form tool settings.

## Step 12: Add reCAPTCHA v3 Last

Only add this after the journey works.

Flow:

```text
Render site key from SettingsPage
JS gets token before submit
POST includes token
Service verifies token using secret from SettingsPage
```

Verify:

- Empty secret means captcha is skipped for local dev.
- With secret configured, every step validates.

## Step 13: Add More Operators

After basic rules work, add:

```csharp
Contains,
NotContains,
IsAnyOf,
IsNotAnyOf
```

For `IsAnyOf`, let editors enter:

```text
Business,Personal,Enterprise
```

## Recommended Build Order

1. CMS starts with Forms installed.
2. Dummy container renders normal Forms elements.
3. Dummy question element appears in CMS.
4. DDS state saves current element.
5. One-question-at-a-time rendering works.
6. Submission answer save/read works.
7. Next-question rules work.
8. Back button works.
9. Branch reset works.
10. Summary resume works.
11. SettingsPage tool resolution works.
12. reCAPTCHA works.
13. More operators and polish.

This keeps the architecture small and lets you test each piece before adding the next.
