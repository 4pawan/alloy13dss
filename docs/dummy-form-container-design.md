# Dummy Form Container Design

`DummyFormContainerBlock` extends Optimizely Forms instead of replacing it. Normal Forms elements still own rendering, validation, and submission storage. The custom layer decides which `DummyQuestionElementBlock` is currently active, validates reCAPTCHA v3 for each step from `SettingsPage`, and persists journey navigation state in DDS.

## Content Model

- `DummyFormContainerBlock : FormContainerBlock`
- `DummyQuestionElementBlock : TextboxElementBlock`
- `DummyRecaptchaV3ElementBlock : HiddenElementBlock`
- `SettingsPage` stores the shared reCAPTCHA v3 site key, secret key, and score threshold.

Editors add `DummyQuestionElementBlock` items directly to the form's normal elements area:

```text
DummyFormContainerBlock
  Forms Elements Area
    Q1 [DummyQuestionElementBlock]
    Q2 [DummyQuestionElementBlock]
    Q3 [DummyQuestionElementBlock]
```

Each `DummyQuestionElementBlock` has a `Rules` list. These rules check previously submitted responses by `BranchingKey` and decide whether the current question should be shown.

Once the summary question has been visited, a later resume can return to it because it is the last visited element.

## Runtime State

`DummyFormJourneyState` is stored in DDS by Forms submission id:

- `SubmissionId`
- `CurrentElement`
- `VisitedElements`
- `UpdatedUtc`

The submission id is the resume key. The state record tracks visited elements so users can move back through already visited questions.

## Step Submission Flow

1. First render shows the first `DummyQuestionElementBlock` in the Forms elements area.
2. User submits the current question.
3. Validate the current visible Forms element using the built-in Forms validation pipeline.
4. Validate reCAPTCHA v3 on every step using the page's selected `SettingsPage`.
5. Save the current question value into the Optimizely Forms submission.
6. Scan the next `DummyQuestionElementBlock` items in order.
7. For each next question, evaluate that question's own `Rules`.
8. Skip questions whose rules do not match.
9. Render the first matching question.

If the journey is later resumed after completion, the service restores the last visited element from `VisitedElements`.

## Submission Answer Mapping

Answers are stored in Optimizely Forms submissions by element field name. At runtime, `DummyFormSubmissionAnswerStore` reads the submission and maps each `DummyQuestionElementBlock.BranchingKey` to the corresponding stored field value so visibility rules can use editor-friendly source keys.
