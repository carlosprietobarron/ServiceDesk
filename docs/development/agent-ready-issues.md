# Agent-ready GitHub issue template

## Purpose

Use **Agent-ready development task** when opening an implementation task intended for a developer, GitHub Copilot or Codex. The issue form lives at `.github/ISSUE_TEMPLATE/agent-ready-task.yml`.

The form is for **planning**; creating an issue does not start an agent, create a branch, assign work or run tests. Follow `.github/copilot-instructions.md` and the task's acceptance criteria before implementing.

## How to use

1. Open the repository **Issues** tab, select **New issue**, then select **Agent-ready development task**.
2. Replace the proposed title prefix with a specific action and component, e.g. `[Task] Validate ticket priority values`.
3. Fill in the objective and current behavior with links to relevant files or requirements.
4. Define what is in scope, out of scope and which architectural/security constraints apply.
5. Identify likely components and test locations, noting assumptions.
6. Write checkable acceptance criteria as `- [ ]` entries, including failure cases when appropriate.
7. Describe concrete validation commands and prerequisites; distinguish planned commands from actual execution.
8. Record blockers or write `None`, then confirm the readiness checkboxes and submit.
9. A developer or agent reviews the issue, works on a task branch, and submits a PR to `main`. Report test results honestly and respect branch protection.

## Agent execution contract

- Inspect current code and relevant design documents before coding. Do not assume backlog features are already implemented.
- Implement only the scope specified by the issue. Ask for clarification or document blockers if material choices are unresolved.
- Keep Domain independent of infrastructure, follow the Application/Infrastructure/API layering, and preserve TypeScript and security conventions.
- Never commit secrets, destructive maintenance steps or production database modifications.
- Verify only applicable commands. For backend tasks use `dotnet restore ServiceDesk.slnx`, `dotnet build ServiceDesk.slnx --no-restore --configuration Release` and appropriate unit tests. For frontend tasks use `npm ci`, `npm run lint` and `npm run build` in `src/frontend`.
- For persistence changes, require a nonproduction PostgreSQL database before running migration/integration tests.
- Open a PR with `Closes #<issue>`, changed behavior, checks actually run, skipped checks and limitations. Merge only after required checks/review.

## Example issue

**Title:** `[Task] Validate ticket priority values in Domain`

**Objective:** Ensure a ticket cannot be created with a priority value outside the supported domain set.

**Context:** The Domain project lives under `src/services/TicketService/`. Inspect existing entities and value objects first; do not assume the current model already has a Priority type.

**In scope:** Review the domain model, apply the smallest validation change consistent with the existing design, add focused Domain unit tests.

**Out of scope and constraints:** No API endpoints, database migration, SLA processing, or UI. Domain must not depend on EF Core.

**Likely affected components:** `ServiceDesk.Tickets.Domain` and `tests/ServiceDesk.Tickets.Domain.Tests` (exact files to confirm).

**Acceptance criteria:**
- [ ] Valid supported priority values are accepted.
- [ ] Unsupported values are rejected with a clear domain failure.
- [ ] Domain tests cover valid and invalid values.
- [ ] Existing Domain tests remain passing.

**Verification plan:** `dotnet restore ServiceDesk.slnx`, `dotnet build ServiceDesk.slnx --no-restore --configuration Release`, `dotnet test tests/ServiceDesk.Tickets.Domain.Tests/ServiceDesk.Tickets.Domain.Tests.csproj --no-build --configuration Release`. These are proposed checks, not executed results.

**Dependencies:** Confirm priority definitions in the approved requirements before coding.

This example is illustrative and does **not** authorize implementing priority validation as part of issue #20.

## Manual acceptance check for issue #20

After the branch is merged, open **Issues > New issue** and verify the template appears, the expected fields render, and submitting with missing required fields is blocked. Create a disposable example issue if desired, then close it. This live UI behavior cannot be fully validated from the file alone.
