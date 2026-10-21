# ServiceDesk - Docker Compose Development Environment

## Overview

The local development environment uses Docker Compose to run:

- PostgreSQL 17
- ServiceDesk Tickets API (.NET 10)

The API connects to PostgreSQL through the internal Docker network.

## Prerequisites

- Docker Desktop with Docker Compose
- Git
- A local .env file containing:
  - POSTGRES_DB
  - POSTGRES_USER
  - POSTGRES_PASSWORD
  - POSTGRES_PORT (optional, defaults to 5432)

Never commit the .env file or database credentials.

## Build and start

From the repository root:

    docker compose up -d --build

## Check service status

    docker compose ps

Expected services:

- postgres: Up (healthy)
- api: Up

## Verify the API

    Invoke-RestMethod -Uri "http://localhost:8080/weatherforecast" -Method Get

The endpoint returns sample weather data.

## View logs

    docker compose logs --tail=30 api

    docker compose logs --tail=30 postgres

## Stop services

    docker compose down

This stops and removes containers but preserves the PostgreSQL data volume.

## Restart services

    docker compose up -d

## Rebuild the API

    docker compose up -d --build api

## Database persistence

PostgreSQL uses the named volume postgres_data.

WARNING: Running docker compose down -v deletes the database volume and its stored data.

## Database connection

Inside Docker, the API uses:

    Host=postgres;Port=5432

The remaining connection parameters are supplied through environment variables.

## Integration testing

Integration tests must use an accessible PostgreSQL instance with the required EF Core migrations applied.

The TicketPersistenceTests suite verifies ticket creation, persistence, retrieval, and history.

## Security

- The API runs as the non-root app user.
- The Dockerfile uses a multi-stage build.
- .dockerignore excludes build artifacts and local secrets.
- Credentials are not stored in the Dockerfile.
- Local environment variables are not a production secrets-management solution.
