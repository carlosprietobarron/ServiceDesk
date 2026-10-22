
# ServiceDesk - Configuration and Secrets Strategy

## Purpose

Define consistent rules for managing application configuration and sensitive information across development and production environments.

## General principles

- Never commit passwords, API keys, access tokens or private certificates.
- Keep configuration separate from application code.
- Use environment-specific configuration where appropriate.
- Never include production secrets in Docker images.
- Apply the principle of least privilege.
- Rotate credentials if accidental exposure occurs.

## ASP.NET Core configuration

The Tickets API uses the standard ASP.NET Core configuration providers.

Configuration is loaded from:

1. appsettings.json
2. appsettings.{Environment}.json
3. .NET User Secrets in Development
4. Environment variables

Later providers override earlier providers when they define the same key.

Command-line configuration arguments may also override these values.

## Local development

Windows and Ubuntu developers should use .NET User Secrets for sensitive values.

Example:

    dotnet user-secrets set "ConnectionStrings:Tickets" "<local-connection-string>" --project src/services/TicketService/ServiceDesk.Tickets.Api

User Secrets are stored outside the repository.

They are intended for development and are not encrypted secret storage.

## Docker Compose development

Docker Compose uses a local .env file to supply database configuration values.

Required variables:

- POSTGRES_DB
- POSTGRES_USER
- POSTGRES_PASSWORD

Optional variable:

- POSTGRES_PORT

The .env file must never be committed.

The public .env.example file documents the expected variables without providing real credentials.

The API receives its connection string through ConnectionStrings__Tickets.

Within Docker Compose, the PostgreSQL hostname is postgres rather than localhost.

## Git and Docker protection

.gitignore excludes local environment files and secret configuration overrides.

.dockerignore excludes local environment files, build artifacts and other unnecessary files from the Docker build context.

These rules reduce accidental exposure but are not substitutes for secret scanning and access controls.

## Production environments

Production credentials must be supplied through a secure secret-management mechanism supported by the deployment platform.

Requirements:

- No production credentials in Git.
- No secrets embedded in Docker images.
- Restrict access to authorized services and personnel.
- Support credential rotation.
- Avoid recording sensitive values in application logs.
- Use encrypted connections where appropriate.

The production provider and secret-management service will be selected during deployment planning.

## Verification

Before merging changes:

1. Review staged files for credentials.
2. Confirm that private .env files are ignored.
3. Confirm that .env.example remains versioned.
4. Validate Docker Compose configuration.
5. Verify that the application starts using externally supplied configuration.

Never publish the output of commands that reveal secret values.

## Secret exposure response

If a credential is accidentally committed:

1. Revoke or rotate the exposed credential immediately.
2. Review access logs and possible misuse.
3. Remove the credential from current files.
4. Assess whether repository history requires remediation.
5. Document the incident without reproducing the secret.
