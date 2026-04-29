# app-appvisorpdf-04-fe Specification

## Purpose
Definir e implementar la capa `AppVisorPdfApi` para desacoplar `AppVisorPdf` de la infraestructura HTTP,
usando `Clienteaxios` y el envelope `ApiResponse<T>`, con tipado estricto y tests con mocks.

## Requirements

### Requirement: AppVisorPdf SHALL expose a dedicated API adapter module
El sistema SHALL exponer un adaptador `infrastructure/visorPdfApi.ts` que implementa `AppVisorPdfApi`
sin acoplar UI a endpoints hardcodeados.

#### Scenario: UI does not hardcode endpoints
- **WHEN** `AppVisorPdf` needs PDF/annotations/stamp-config
- **THEN** it SHALL call `AppVisorPdfApi` methods instead of importing axios or hardcoding routes

### Requirement: AppVisorPdfApi SHALL use ApiResponse<T> envelopes
El sistema SHALL retornar `ApiResponse<T>` en todos los m\u00e9todos definidos por contrato.

#### Scenario: Envelope is preserved
- **WHEN** `getAnnotations(documentId)` is called
- **THEN** it SHALL return `Promise<ApiResponse<VisorPdfAnnotationsPayloadV1>>`

### Requirement: AppVisorPdfApi SHALL propagate auth/permission errors
El adaptador SHALL propagar errores HTTP 401/403 sin ocultarlos, para permitir manejo centralizado.

#### Scenario: Unauthorized is propagated
- **WHEN** backend responds with 401
- **THEN** the promise SHALL reject with the original error (or equivalent) without converting it to a success envelope

#### Scenario: Forbidden is propagated
- **WHEN** backend responds with 403
- **THEN** the promise SHALL reject with the original error (or equivalent)

### Requirement: AppVisorPdfApi SHALL be strictly typed
El sistema SHALL mantener tipado estricto y SHALL avoid `any`.

#### Scenario: Strict typing
- **WHEN** the module is compiled with TypeScript strict settings
- **THEN** there SHALL be no implicit-any or explicit `any` usage

### Requirement: AppVisorPdfApi SHALL be unit tested via axios client mocks
El sistema SHALL incluir tests unitarios que mockean `Clienteaxios` para validar requests y respuestas.

#### Scenario: Save annotations sends payload
- **WHEN** `saveAnnotations(documentId, payload)` is called
- **THEN** it SHALL send the same payload as request body

#### Scenario: 401/403 errors are surfaced
- **WHEN** the underlying client rejects with 401/403
- **THEN** the API method test SHALL assert that the error is propagated

