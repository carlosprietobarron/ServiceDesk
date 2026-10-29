# Coding agent v0.1 — GitHub Issue to Pull Request

## Purpose

ServiceDesk uses **GitHub Copilot cloud coding agent** as an optional, human-supervised implementation assistant. A scoped issue can be assigned to Copilot, which works in its own branch and proposes a pull request. The human owner reviews the changes and existing CI before merging. This configuration does **not** automatically assign issues, merge pull requests, deploy, or create a proprietary agent service.

GitHub Copilot is a separate GitHub feature with its own plan eligibility, repository/organization policy and usage limits. Access through ChatGPT or GitHub alone does not imply Copilot coding-agent access.

## Files and responsibilities

| Path | Purpose |
| --- | --- |
| `.github/copilot-instructions.md` | Existing architecture, code style, secrets and testing instructions. Read before coding. |
| `.github/workflows/copilot-setup-steps.yml` | Prepares the Copilot Actions-based agent environment with .NET 10, local .NET tools, Node.js 22 and npm packages. |
| `.github/ISSUE_TEMPLATE/coding-agent-task.yml` | Optional structured scope and acceptance criteria for an agent-ready issue. |
| `.github/workflows/ci.yml` | Existing authoritative CI; unchanged by task 21. |
| `docs/development/main-branch-protection.md` | Human approval and branch protection reference. |

The setup workflow is **not a coding-agent implementation or invocation mechanism**. GitHub recognizes the special workflow when it is present on the repository default branch. It applies to GitHub Actions-based Copilot agent environments, not necessarily other sandbox execution modes.

## Prerequisites

1. The repository must have GitHub Issues and Actions enabled.
2. The user must have eligible GitHub Copilot cloud-agent access and the agent must be enabled for this repository by applicable account, organization and enterprise settings.
3. The setup workflow and repository instructions must be merged into `main` before expecting them to affect subsequent Copilot assignments.
4. CI checks and branch-protection rules must remain enforced. Never put real secrets in issues or prompts.

## End-to-end runbook

1. From GitHub, create a small non-production issue using **Issues → New issue → Coding agent task**. State the objective, code locations, explicit acceptance criteria, validation commands and exclusions. Break up large tasks.
2. Open the issue, click **Assignees**, and choose **Copilot**. If Copilot is not offered, verify the prerequisites rather than assigning an arbitrary bot. Choose `main` as starting branch if GitHub offers that option, and confirm assignment.
3. GitHub starts the agent session. Inspect its logs and the branch/pull request that Copilot creates. Its ability to install packages or run tests depends on the environment, network and available permissions.
4. Review the PR for scope, architecture boundaries, database migrations, secrets, missing tests and accidental changes. Do not assume generated code is correct.
5. Check the existing **ServiceDesk CI** result on the PR. Run extra integration or frontend checks if the issue changes those areas; the current CI does not validate frontend builds.
6. Request changes through PR comments when necessary. Merge only after required checks and human review pass. Close the issue after verifying the linked outcome.

### Suggested first smoke test

Create a test issue asking the agent to correct or clarify one sentence in an existing `docs/development/` Markdown file, with the acceptance criteria that no application code changes and a PR is opened. This tests the assignment-to-PR path without modifying database behavior. Do not assign issue #21 itself as the smoke test.

## Setup workflow verification

After opening the task-21 PR, verify that GitHub Actions runs the **Copilot Setup Steps** job on the changed workflow and confirm each step completed. After merging to `main`, it can also be started manually from **Actions → Copilot Setup Steps → Run workflow**. Separately confirm that a test issue can be assigned to Copilot and results in a pull request. Passing setup alone **does not** prove the full Issue → PR integration.

Checks for a development environment (the agent may run them as appropriate):

```sh
dotnet restore ServiceDesk.slnx
dotnet build ServiceDesk.slnx --no-restore --configuration Release
dotnet test tests/ServiceDesk.Tickets.Domain.Tests/ServiceDesk.Tickets.Domain.Tests.csproj --no-build --configuration Release
dotnet test tests/ServiceDesk.Tickets.Application.Tests/ServiceDesk.Tickets.Application.Tests.csproj --no-build --configuration Release
cd src/frontend
npm run lint
npm run build
```

Integration tests require isolated PostgreSQL and configured non-production credentials; do not run database migrations against production. The setup workflow intentionally does not start a database, execute migrations or run test suites: it prepares dependencies, while CI verifies backend behavior.

## Security and operational limits

- Only assign trusted, scoped issues. Issue content and repository files can influence the agent; review any instructions or commands the agent follows.
- The setup job uses `contents: read` only and no privileged or secret-bearing triggers. Do not add `pull_request_target`, unattended merge, broad write permissions or production credentials.
- The agent must not access real customer data, production databases or deployment secrets.
- Changes to `main` are made through a PR and required review/checks; keep separation of duties.
- Cloud-agent usage may consume GitHub Copilot premium requests and Actions minutes according to the account plan.

## Troubleshooting

| Symptom | Action |
| --- | --- |
| Copilot absent in assignees | Check Copilot plan, permissions and whether cloud agent is enabled for repository. |
| Setup not used | Confirm the special workflow exists in default branch and the session uses an Actions-based agent environment. |
| Setup failing | Read Actions step logs, check .NET/Node versions and npm lockfile. |
| Agent PR fails CI | Examine the failing job and request a fix in the PR. Do not bypass branch protection. |
| No frontend test job | Run `npm run lint` and `npm run build` when frontend is changed; task 21 does not alter CI. |

## References

- GitHub Docs — [Kick off a task with Copilot agents](https://docs.github.com/en/copilot/how-tos/copilot-on-github/use-copilot-agents/kick-off-a-task).
- GitHub Docs — [Configure the development environment](https://docs.github.com/en/copilot/how-tos/copilot-on-github/customize-copilot/customize-cloud-agent/customize-the-agent-environment).
- ServiceDesk repository instructions — `.github/copilot-instructions.md`.
