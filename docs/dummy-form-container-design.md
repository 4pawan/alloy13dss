# Dummy Form Container Design

`DummyFormContainerBlock` extends Optimizely Forms instead of replacing it. Normal Forms elements still own rendering, validation, and submission storage. The custom layer only decides which element is currently active, validates reCAPTCHA v3 for each step, and persists journey navigation state in DDS.

## Content Model

- `DummyFormContainerBlock : FormContainerBlock`
- `DummyQuestionElementBlock : TextboxElementBlock`
- `DummyRecaptchaV3ElementBlock : HiddenElementBlock`

Editors configure an ordered `JourneyRules` list on the container. Each rule points to a Forms element, has optional show conditions, branches, and a default next element. This uses `PropertyList<T>` backing types so editors get collection editing rather than JSON.

## Runtime State

`DummyFormJourneyState` is stored in DDS by Forms submission id:

- `SubmissionId`
- `FormContentLink`
- `CurrentElement`
- `VisitedElements`
- `UpdatedUtc`

The submission id is the resume key. The state record tracks visited elements so users can move back and forward through already visited questions. Answers stay in Optimizely Forms submission data and are loaded from there when branch conditions are evaluated. When a branch target changes the path, future visited elements are removed from DDS and the corresponding abandoned answers should be cleared from the Forms submission.

## Step Submission Flow

1. Resolve or create the Forms submission id.
2. Validate the current visible Forms element using the built-in Forms validation pipeline.
3. Validate `DummyRecaptchaV3ElementBlock` on every step.
4. Save the current question value into the Forms submission.
5. Evaluate branches for the current rule.
6. If a branch matches, reset future visited elements and clear old answers for the abandoned path from the Forms submission.
7. Resolve the next eligible element. If its conditions fail, continue checking subsequent rules until one is eligible.
8. Render only that element.

## Next Implementation Point

The remaining integration should be a custom controller/view component for `DummyFormContainerBlock` that derives from, wraps, or mirrors `FormContainerBlockController`. It should keep the default Forms hidden fields, anti-forgery token, submission id handling, and resource registration intact, while filtering rendered elements to the active `CurrentElement`.

Avoid storing journey state only in session. Session can improve UX, but DDS remains the source of truth because users may resume by submission id.
