## Purpose

Provides a repeatable local development workflow for the legacy ASP.NET Framework WebForms application, focused on validating environment readiness, building the project, and confirming the configured IIS site responds.

## ADDED Requirements

### Requirement: Development script validates prerequisites
The development workflow SHALL validate required local prerequisites before attempting to build or check the web application.

#### Scenario: Required tooling is available
- **WHEN** the developer runs the development workflow script
- **THEN** the workflow confirms MSBuild is available and the IIS web publishing service can be inspected

#### Scenario: Required project files are present
- **WHEN** the developer runs the development workflow script from the project repo
- **THEN** the workflow confirms the solution or project file, `Web.config`, and package metadata exist before continuing

### Requirement: Development script builds the legacy web application
The development workflow SHALL build the ASP.NET Framework VB.NET web application using the repo's existing Visual Studio/MSBuild project structure.

#### Scenario: Build succeeds
- **WHEN** the project compiles successfully
- **THEN** the workflow reports the build as successful and continues to runtime checks

#### Scenario: Build fails
- **WHEN** MSBuild returns a failure
- **THEN** the workflow stops and surfaces the build output location or error summary

### Requirement: Development script validates local IIS readiness
The development workflow SHALL validate that the configured local IIS application endpoint is reachable after a successful build.

#### Scenario: IIS endpoint responds
- **WHEN** the configured local application URL responds within the allowed retry window
- **THEN** the workflow reports the application as ready and displays the URL

#### Scenario: IIS endpoint does not respond
- **WHEN** the configured local application URL does not respond within the allowed retry window
- **THEN** the workflow reports likely IIS, binding, certificate, virtual directory, or application pool causes

### Requirement: Development script avoids React-specific assumptions
The development workflow SHALL NOT require Vite, React, `node_modules`, or a .NET Core executable API to run this legacy application.

#### Scenario: React tooling is absent
- **WHEN** the repo does not contain React/Vite tooling
- **THEN** the workflow still supports validation, build, and IIS readiness checks for the ASP.NET Framework application
