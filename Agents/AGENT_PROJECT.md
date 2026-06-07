## Project Description

- **Purpose**: Self-hosted Twitch chat bot built with .NET; learning project for chat automation systems
- **Core Features**: Customizable text commands, message responses, variable management
- **Layered Architecture**:
  - **Bot Core** (ComfyBot.Bot): Twitch chat integration and message processing
  - **Application Layer** (ComfyBot.Application): Business logic (chat response management)
  - **Data Layer** (ComfyBot.Data): Data persistence
  - **Common Layer** (ComfyBot.Common): Shared utilities
- **Modernization**: Gateway service acts as centralized hub
- **Gateway Responsibilities**: Connects to Twitch chat, distributes messages via SignalR, maintains message history in Redis

## Current State

- **Target Framework**: .NET 10 across all projects
- **Architecture**: Gateway-based pattern decouples message handling from bot logic
- **Gateway Service** (ComfyBot.Gateway):
  - Central hub for Twitch chat connectivity
  - Uses Redis for message caching
  - Uses SignalR for real-time client communication
- **Code Patterns**:
  - Outcome pattern throughout application layer for explicit error handling
  - Dependency injection via custom modules
  - Clean separation of concerns
- **Testing**: Comprehensive test coverage with separate test projects (Application, Data, Bot layers)
- **Data Layer**:
  - PostgreSQL as primary database (replaced SQLite)
  - Entity Framework Core with Npgsql provider
  - EF Core migrations configured for PostgreSQL
- **Frontend Styling**:
  - `variables.css` file with CSS custom properties
  - Design tokens for colors, typography, spacing
  - `app.css` refactored to use CSS variables for consistency and maintainability

## Deployment and Infrastructure

- **Containerization**: Both ComfyBot.Gateway and ComfyBot.Application have Dockerfiles
- **Orchestration**: Shared `docker-compose.yml` at project root
- **Services in Docker Compose**:
  - PostgreSQL: Data persistence
  - Redis: Message caching
  - ComfyBot.Gateway: Chat gateway service
  - ComfyBot.Application: Main application service
- **Configuration**: Environment variables only (no hardcoded values)
- **Runtime Setup**:
  - Optional `.env` file to override default settings
  - Complete setup instructions in `DOCKER_COMPOSE_SETUP.md`
  - Single-command startup: `docker-compose up -d`
- **Communication**: Services communicate via container networking

## Database Configuration

- **ORM**: Entity Framework Core
- **Database Provider**: Npgsql.EntityFrameworkCore.PostgreSQL v8.0.1
- **Connection String Format**: `Host={host};Port={port};Database={database};Username={username};Password={password}`
- **Development Defaults**: see config files for local development settings
- **Configuration Method**: appsettings.json files with environment-based overrides
- **Migrations**: Automatically applied on application startup
- **Containerized Deployments**: Connection strings injected via environment variables pointing to Docker Compose PostgreSQL service

