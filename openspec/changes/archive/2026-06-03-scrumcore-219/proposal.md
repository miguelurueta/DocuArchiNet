## Why

The structure-by-task flow currently normalizes several backend fields but does not formally model or normalize `idRespuestaRadicado`. This creates risk of unsafe access, duplicated casing handling, and future hardcodes in hooks or UI consumers.

## What Changes

- Add typed support for backend casing variants of `idRespuestaRadicado` in the structure response DTO.
- Normalize all supported backend variants into a single frontend field: `idRespuestaRadicado`.
- Keep normalization centralized in `mapEstructuraRespuesta`.
- Preserve existing normalized fields such as `Radicado`, `Destinatario`, and `TramiteDocumento`.
- Keep fallback as `undefined` when the backend does not provide the field.
- Add mapper and hook tests covering supported casing variants and legacy compatibility.
- Add technical documentation for architecture, backend integration, tests, metadata, fallback, and compatibility.
- No UI changes, endpoint changes, backend contract changes, or business logic changes.

## Capabilities

### New Capabilities

<!-- None. This change extends an existing Gestion Correspondencia flow. -->

### Modified Capabilities

- `gestion-correspondencia`: normalize and type `idRespuestaRadicado` in the structure-by-task flow while preserving existing DTO and frontend model compatibility.

## Impact

- Affected types:
  - `src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts`
- Affected adapter:
  - `src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts`
- Affected hook:
  - `src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts`
- Affected tests:
  - `src/modules/gestionCorrespondencia/tests/*`
- Affected documentation:
  - `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/normalizatipado/`
- No backend API endpoint changes.
- No UI component changes.
- No new dependencies.
