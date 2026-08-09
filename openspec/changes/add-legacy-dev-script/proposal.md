## Why

The legacy GestionDocumental project currently lacks a repeatable development entrypoint comparable to the React repo's `scripts/dev-full.ps1`. Developers need a repo-local script that validates the ASP.NET Framework/IIS environment, builds the VB.NET web application, and reports clear readiness errors before opening or checking the local site.

## What Changes

- Add a development workflow script adapted to this ASP.NET WebForms/VB.NET project.
- Validate required local tooling and runtime assumptions before attempting to run the app.
- Build the solution or project with MSBuild using the existing .NET Framework 4.6.1 web application structure.
- Validate IIS availability and the configured local application URL.
- Provide clear diagnostics for missing IIS configuration, package dependencies, database/DSN prerequisites, or certificate/binding issues.
- Avoid React/Vite/.NET Core assumptions from the source script.

## Capabilities

### New Capabilities
- `legacy-dev-environment`: Defines the expected local development script behavior for the ASP.NET Framework WebForms project.

### Modified Capabilities

## Impact

- Affected files are expected under `scripts/` and possibly project-local documentation.
- The script will interact with local MSBuild, IIS/W3SVC, the configured IIS application URL, NuGet/package state, and `Web.config` settings.
- No application runtime behavior, public APIs, or database schema should change.
