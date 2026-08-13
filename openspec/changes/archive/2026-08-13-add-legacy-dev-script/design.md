## Context

The target project is an ASP.NET Framework 4.6.1 Web Application written in VB.NET. The `.vbproj` declares `UseIIS=True` and the configured local URL is `https://localhost/GestionDocumental-Docuarchi.net`. The project has `packages.config`, dependencies under `..\packages`, compiled binaries in `bin`, and runtime configuration in `Web.config`.

The source React script starts a published .NET API DLL and then Vite. Those assumptions do not apply here: this project is hosted by IIS and built with MSBuild/Visual Studio web application targets.

## Goals / Non-Goals

**Goals:**
- Provide a single script entrypoint for local development checks.
- Validate local prerequisites before build/runtime checks.
- Build the existing solution or project through MSBuild.
- Check whether the IIS-hosted application responds at the configured URL.
- Produce actionable diagnostics instead of generic PowerShell failures.

**Non-Goals:**
- Do not migrate the application to .NET Core.
- Do not introduce React, Vite, npm, or Node-based runtime requirements.
- Do not change application source behavior, database schema, authentication, or IIS production settings.
- Do not automatically create or modify IIS sites unless a later explicit setup mode is added.

## Decisions

1. Use a repo-local PowerShell script under `scripts/`.

   Rationale: the React repo already uses this convention and it keeps developer commands discoverable. Alternative considered: a `.bat` wrapper only. PowerShell is better for service checks, retries, and structured error handling.

2. Treat IIS as the runtime host.

   Rationale: the project file is configured for IIS and uses an HTTPS localhost application path. Alternative considered: launching `dotnet` or Vite, but this project does not expose that runtime model.

3. Use MSBuild as the build mechanism.

   Rationale: this is a legacy Visual Studio web application targeting .NET Framework 4.6.1. Alternative considered: `dotnet build`, but SDK-style assumptions are unreliable for this project type.

4. Make the first version diagnostic-first.

   Rationale: configuring IIS requires machine-specific state and often administrator permissions. The safer baseline is to detect and explain missing site/app pool/binding setup, then add opt-in setup automation later if needed.

5. Use the configured URL as default but allow overrides.

   Rationale: the `.vbproj` currently points to `https://localhost/GestionDocumental-Docuarchi.net`, but individual machines may use different bindings or ports. Parameters such as `-Url`, `-Configuration`, and `-SkipBuild` keep the script useful without editing it.

## Risks / Trade-offs

- IIS site not configured -> Report the expected URL and recommended IIS application path; keep setup automation out of the default path.
- HTTPS certificate or binding failure -> Catch web request errors and include certificate/binding as likely causes.
- Missing NuGet restore tooling -> Validate `packages.config` and `..\packages`; report restore guidance when assemblies are missing.
- Database/ODBC unavailable -> A page request may fail even when IIS is correct; report `Web.config` connection prerequisites as a likely cause.
- MSBuild path varies by Visual Studio installation -> Resolve from PATH first and optionally support `vswhere` discovery later.

## Migration Plan

1. Add the script without changing application code.
2. Run it locally from the project root.
3. Adjust diagnostics based on actual machine failures.
4. Optionally add `-SetupIIS` in a later change if manual IIS setup remains a repeated blocker.

Rollback is removing the new script/documentation; application behavior is unaffected.
