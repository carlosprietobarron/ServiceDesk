# ServiceDesk repository instructions

## Project and scope

- ServiceDesk is an IT ticket management MVP. Implement only the requested issue scope.
- Inspect the relevant source and documentation before editing. Do not treat planned features as implemented.
- The backend targets .NET 10 with nullable reference types and implicit usings enabled. Persistence uses EF Core 10 and Npgsql with PostgreSQL 17.
- The frontend is React with TypeScript and Vite, managed with npm in `src/frontend`.
- Application still contains a scaffold; the API currently exposes the sample weather endpoint. Do not assume ticket endpoints, authentication, authorization or SLA processing already exist.
- Follow existing patterns and package versions. Do not introduce frameworks, packages or architectural changes without a task-specific reason.

## Repository map and architecture

- `ServiceDesk.slnx` is the backend solution.
- Backend projects are under `src/services/TicketService/`.
- `ServiceDesk.Tickets.Domain` owns entities, value objects and business invariants. Keep it independent of Application, Infrastructure, ASP.NET Core and EF Core.
- `ServiceDesk.Tickets.Application` owns use cases and the abstractions they require. It references Domain; do not add Infrastructure or Api dependencies.
- `ServiceDesk.Tickets.Infrastructure` implements persistence and external integrations. It references Domain and Application.
- `ServiceDesk.Tickets.Api` owns HTTP handling and application composition. Its references to Application and Infrastructure are intentional; keep business rules out of endpoint handlers.
- `tests/` contains Domain, Application and IntegrationTests projects.
- Read relevant baseline documents in `docs/` for requirements and design. If they conflict with current code or the requested task, identify the discrepancy rather than silently changing the architecture.

## Backend conventions and persistence

- Match surrounding C# style, including file-scoped namespaces, PascalCase types and members, and camelCase parameters and local variables.
- Preserve nullability checks and domain validation. Follow existing encapsulation with private setters and read-only collection exposure.
- Keep existing value objects such as UserId and TicketNumber rather than replacing them with primitive fields.
- Ticket timestamps use UTC. Preserve the creation history and actor information when changing ticket behavior.
- EF Core context, mappings and migrations belong in Infrastructure/Persistence. Keep relational mapping in the existing configuration classes.
- Generate and review migrations for schema changes; do not rewrite applied migrations or hand-edit generated snapshots to bypass migration generation.
- Use asynchronous I/O where supported; avoid blocking asynchronous calls.

## Frontend conventions

- Follow the existing TypeScript, React function-component and CSS patterns in `src/frontend/src`; pages currently live in `src/pages` relative to the frontend directory.
- Preserve TypeScript checks and ESLint rules. Avoid `any` or disabling checks to hide errors.
- Use the existing npm scripts and lockfile. Do not assume a frontend test runner exists; none is currently configured.
- Never put secrets in frontend source or Vite client environment variables.

## Configuration and secrets

- Follow `docs/development/configuration-and-secrets.md`.
- Local API development uses User Secrets for `ConnectionStrings:Tickets`. Keep the existing API UserSecretsId.
- Compose uses an ignored local `.env`; `.env.example` is the versioned variable template.
- The API container receives `ConnectionStrings__Tickets` through its environment. The PostgreSQL hostname inside Compose is `postgres`; host-run tools use the published host address and port.
- User Secrets on the host are not automatically available inside the API container.
- Never commit real passwords, tokens, connection strings containing real credentials, private environment files or secret-bearing command output. Use placeholders in examples.
- Do not embed secrets in Docker images or log them.
- Do not remove Docker volumes or reset a database as a routine troubleshooting step.

## Validation

Run commands from the repository root unless stated otherwise. Report which checks actually ran and any missing prerequisites; never claim an unexecuted check passed.

Backend restore and build:

```sh
dotnet restore ServiceDesk.slnx
dotnet build ServiceDesk.slnx --no-restore --configuration Release
```

Unit tests after the Release build:

```sh
dotnet test tests/ServiceDesk.Tickets.Domain.Tests/ServiceDesk.Tickets.Domain.Tests.csproj --no-build --configuration Release
dotnet test tests/ServiceDesk.Tickets.Application.Tests/ServiceDesk.Tickets.Application.Tests.csproj --no-build --configuration Release
```

For persistence changes, use a development or test PostgreSQL database with the connection configured through User Secrets or environment variables. Confirm the target before applying migrations. The integration test reads User Secrets followed by environment variables, so environment values override secrets.

```sh
dotnet tool restore
dotnet ef database update --project src/services/TicketService/ServiceDesk.Tickets.Infrastructure --startup-project src/services/TicketService/ServiceDesk.Tickets.Api --configuration Release --no-build
dotnet test tests/ServiceDesk.Tickets.IntegrationTests/ServiceDesk.Tickets.IntegrationTests.csproj --no-build --configuration Release
```

- The existing persistence test uses a transaction and rolls it back. Preserve test isolation and never run integration tests against production.
- Add meaningful tests for changed behavior in the corresponding project. Domain and Application unit tests should not require PostgreSQL.
- For frontend changes, run `npm ci`, `npm run lint` and `npm run build` from `src/frontend`.
- CI is defined in `.github/workflows/ci.yml`. It builds the backend, runs unit tests, applies migrations to its PostgreSQL service and runs integration tests. It does not currently validate the frontend.

## Delivery and references

- Use a task branch and a pull request to main, following `docs/development/main-branch-protection.md`. Do not push directly to main or bypass required checks.
- Keep changes focused. Update relevant documentation when behavior or setup changes.
- In the PR, describe the resulting behavior, validation performed and material limitations.
- Development references: `docs/development/configuration-and-secrets.md`, `docs/development/docker-compose.md`, `docs/postgresql-development.md` and `docs/local-environment.md`.
