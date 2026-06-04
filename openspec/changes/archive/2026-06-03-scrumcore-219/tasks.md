## 1. Types

- [x] 1.1 Extend `SolicitaEstructuraRespuestaBackendItem` with optional `idRespuestaRadicado`, `IdRespuestaRadicado`, `ID_RESPUESTA_RADICADO`, and `id_respuesta_radicado` fields typed as `string | number`.
- [x] 1.2 Extend `GestionRespuestaEstructuraRespuesta` with optional `idRespuestaRadicado?: string | number`.

## 2. Adapter Normalization

- [x] 2.1 Update `mapEstructuraRespuesta` to resolve supported backend variants into normalized `idRespuestaRadicado`.
- [x] 2.2 Keep `idRespuestaRadicado` as `undefined` when no supported backend variant is present.
- [x] 2.3 Preserve existing mapping behavior for `Radicado`, `Destinatario`, and `TramiteDocumento`.
- [x] 2.4 Apply deterministic precedence when multiple variants exist: `idRespuestaRadicado`, `IdRespuestaRadicado`, `ID_RESPUESTA_RADICADO`, `id_respuesta_radicado`.

## 3. Hook Integration

- [x] 3.1 Update `useEstructuraRespuestaIdTarea` so the mapper receives a typed `SolicitaEstructuraRespuestaBackendItem`.
- [x] 3.2 Remove unsafe `any` usage from the structure mapping path without changing query behavior.
- [x] 3.3 Ensure the hook exposes only the normalized frontend model to consumers.
- [x] 3.4 Verify no component or hook consumer resolves backend casing variants manually.
- [x] 3.5 Confirm current hook states (`loading`, `fetching`, `error`, `isEmpty`, `isEmptyLatched`, `resolved`) keep the same behavior.

## 4. Tests

- [x] 4.1 Add mapper tests for `idRespuestaRadicado`.
- [x] 4.2 Add mapper tests for `IdRespuestaRadicado`.
- [x] 4.3 Add mapper tests for `ID_RESPUESTA_RADICADO`.
- [x] 4.4 Add mapper tests for `id_respuesta_radicado`.
- [x] 4.5 Add mapper fallback test proving the normalized value is `undefined` when absent.
- [x] 4.6 Add mapper precedence test for DTOs containing multiple supported variants.
- [x] 4.7 Add compatibility tests proving `Radicado`, `Destinatario`, and `TramiteDocumento` mappings are unchanged.
- [x] 4.8 Add or update hook tests proving `useEstructuraRespuestaIdTarea` returns normalized `idRespuestaRadicado`.
- [x] 4.9 Add or update hook tests proving legacy payloads without `idRespuestaRadicado` still work without runtime errors.
- [x] 4.10 Add or update regression tests proving current consumers keep operating with the existing structure shape.
- [x] 4.11 Ensure new or updated tests include `[SPEC:SCRUMCORE-219]` in the `describe` or test name.

## 5. Documentation

- [x] 5.1 Create the requested documentation folder under `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/normalizatipado/`.
- [x] 5.2 Add `SCRUMCORE-219-Arquitectura.md` with layers, decisions, Mermaid class diagram, sequence diagram, state diagram, ADR summary, risks, and code traceability.
- [x] 5.3 Add `SCRUMCORE-219-Implementacion-Detallada.md` with modified files, added types, mapping rules, supported variants, fallback strategy, and affected layers.
- [x] 5.4 Add `SCRUMCORE-219-Integracion-BackEnd.md` covering supported casing variants, frontend normalization, and legacy compatibility.
- [x] 5.5 Add `SCRUMCORE-219-Pruebas.md` covering unit, integration, browser interaction, regression, and execution evidence.
- [x] 5.6 Add `SCRUMCORE-219-Metadata.md` with ticket, author, date, version, control changes, and cross references.

## 6. Browser, E2E, and Runtime Quality

- [ ] 6.1 Perform browser interaction validation for the current structure-by-task flow.
- [ ] 6.2 Confirm navigation and current consumers continue working with legacy and normalized data.
- [ ] 6.3 Confirm no new console errors or runtime warnings appear during the validated flow.
- [x] 6.4 Run available E2E or regression coverage for Gestion Correspondencia when present in the repo.

## 7. Validation

- [x] 7.1 Run focused mapper and hook tests.
- [x] 7.2 Run TypeScript validation.
- [x] 7.3 Run available lint validation.
- [x] 7.4 Run available build validation.
- [x] 7.5 Run OpenSpec validation for `scrumcore-219`.
- [x] 7.6 Confirm no UI, endpoint, backend contract, or business logic changes were introduced.
- [x] 7.7 Record test, TypeScript, lint, build, browser, and E2E evidence in `SCRUMCORE-219-Pruebas.md`.
