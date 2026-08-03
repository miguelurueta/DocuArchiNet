## 1. Environment Discovery

- [x] 1.1 Confirm the canonical local URL from `GestionDocumental-Docuarchi.net.vbproj`.
- [x] 1.2 Confirm whether the solution should be built from `..\GestionDocumental-Docuarchi.net.sln` or directly from `GestionDocumental-Docuarchi.net.vbproj`.
- [x] 1.3 Identify required local prerequisites to validate: MSBuild, IIS/W3SVC, project files, package folder, and `Web.config`.

## 2. Script Implementation

- [x] 2.1 Create `scripts/dev-full.ps1` for the legacy ASP.NET Framework application.
- [x] 2.2 Add script parameters for URL, configuration, retry count, retry delay, skip build, and optional browser launch.
- [x] 2.3 Implement MSBuild discovery and build execution with clear failure output.
- [x] 2.4 Implement IIS service and project structure validation.
- [x] 2.5 Implement local endpoint readiness checks against the configured IIS URL.
- [x] 2.6 Add diagnostic messages for IIS setup, HTTPS/certificate, package restore, ODBC/MySQL, and app pool issues.

## 3. Validation

- [x] 3.1 Run the script with build enabled and record whether MSBuild succeeds.
- [x] 3.2 Run the script with `-SkipBuild` to validate IIS/endpoint diagnostics independently.
- [x] 3.3 Verify failure behavior by using an invalid URL override.
- [x] 3.4 Validate the OpenSpec change with `openspec validate add-legacy-dev-script --strict`.

## 4. Documentation

- [x] 4.1 Add concise usage notes for the script parameters.
- [x] 4.2 Document manual IIS prerequisites when the script reports the local site is not configured.
