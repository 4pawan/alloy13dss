# Dummy Form Container Design

`DummyFormContainerBlock` extends Optimizely Forms instead of replacing it. Normal Forms elements still own rendering, validation, and submission storage. The custom layer decides which `DummyQuestionElementBlock` is currently active, validates reCAPTCHA v3 for each step, and persists journey navigation state in DDS.

## Content Model

- `DummyFormContainerBlock : FormContainerBlock`
- `DummyQuestionElementBlock : TextboxElementBlock`
- `DummyRecaptchaV3ElementBlock : HiddenElementBlock`

Editors add `DummyQuestionElementBlock` items directly to the form's normal elements area:

```text
DummyFormContainerBlock
  Forms Elements Area
    Q1 [DummyQuestionElementBlock]
    Q2 [DummyQuestionElementBlock]
    Q3 [DummyQuestionElementBlock]
```

Each `DummyQuestionElementBlock` has a `Rules` list. These rules check previously submitted responses by `BranchingKey` and decide whether the current question should be shown.

Each question can also set `AlwaysRedirectToSourceKey`. When set, submitting that question always jumps to the question whose `BranchingKey` matches the configured value.

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
4. Validate reCAPTCHA v3 on every step.
5. Save the current question value into the Forms submission or answer store.
6. If the current question has `AlwaysRedirectToSourceKey`, redirect to that question.
7. Otherwise scan the next `DummyQuestionElementBlock` items in order.
8. For each next question, evaluate that question's own `Rules`.
9. Skip questions whose rules do not match.
10. Render the first matching question.

## Next Implementation Point

Replace the in-memory answer store with Optimizely Forms submission storage. Answers should be readable by `BranchingKey` so question visibility rules can evaluate submitted responses reliably across app restarts and resumed sessions.
