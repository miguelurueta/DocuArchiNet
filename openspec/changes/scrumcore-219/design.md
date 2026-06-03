## Context

The current structure-by-task flow is implemented through three layers:

- Types in `gestionRespuestaEstructura.types.ts` model the backend item and the normalized frontend structure.
- `mapEstructuraRespuesta` centralizes the existing normalization for `Radicado`, `Destinatario`, and `TramiteDocumento`.
- `useEstructuraRespuestaIdTarea` retrieves the API response, extracts the first payload item, and maps it before exposing `estrucTuraRespuesta`.

The backend can return `idRespuestaRadicado` with multiple casing variants, but the frontend does not currently type or normalize that field. Without a centralized adapter rule, future consumers could start reading backend casing variants directly, increasing coupling to the API payload shape.

This change is intentionally narrow. It only adds typed DTO support, adapter normalization, hook type safety, tests, and documentation. It does not change UI, endpoints, backend contracts, or business behavior.

## Goals / Non-Goals

**Goals:**

- Extend `SolicitaEstructuraRespuestaBackendItem` to accept supported backend variants:
  - `idRespuestaRadicado`
  - `IdRespuestaRadicado`
  - `ID_RESPUESTA_RADICADO`
  - `id_respuesta_radicado`
- Extend `GestionRespuestaEstructuraRespuesta` with `idRespuestaRadicado?: string | number`.
- Normalize the first available supported backend variant into `idRespuestaRadicado`.
- Keep fallback as `undefined` when no supported variant exists.
- Preserve current mapping behavior for `Radicado`, `Destinatario`, and `TramiteDocumento`.
- Ensure `useEstructuraRespuestaIdTarea` exposes only the normalized frontend model.
- Avoid `any` by typing or narrowing the payload before calling `mapEstructuraRespuesta`.
- Add mapper and hook tests for supported variants, fallback, and legacy field compatibility.
- Add technical documentation under the requested Gestion Correspondencia documentation path.

**Non-Goals:**

- No UI changes.
- No endpoint changes.
- No backend contract changes.
- No business rule changes.
- No new dependencies.
- No manual casing resolution in components or consumers.

## Decisions

### Decision 1: Keep casing normalization inside the adapter

`mapEstructuraRespuesta` will be the only place that resolves backend casing variants. The frontend model will expose only `idRespuestaRadicado`.

Alternative considered: resolve casing in `useEstructuraRespuestaIdTarea`.

Rationale: putting casing logic in the hook would duplicate DTO knowledge outside the adapter and make it easier for future consumers to depend on backend casing. The mapper already owns normalization for related fields, so this keeps the boundary consistent.

### Decision 2: Model backend variants as optional DTO fields

The backend DTO type will explicitly list the accepted variants as optional `string | number` fields.

Alternative considered: use an index signature for arbitrary casing variants.

Rationale: an index signature would weaken strict typing and allow unsupported keys to appear valid. Explicit optional fields document the actual compatibility contract and keep TypeScript useful.

### Decision 3: Preserve frontend fallback semantics

If the backend does not provide a supported variant, the normalized field will be omitted as `undefined`.

Alternative considered: use `0`, empty string, or `NaN` as placeholders.

Rationale: placeholder values could be mistaken for valid identifiers. `undefined` accurately represents absence and matches the spec requirement.

### Decision 4: Do not change the existing normalized field casing

The existing frontend model currently exposes `Radicado`, `Destinatario`, and `TramiteDocumento` with their current names. This change will only add `idRespuestaRadicado`; it will not rename existing fields.

Alternative considered: convert all normalized fields to camelCase.

Rationale: renaming existing fields would be a breaking change for current consumers and is outside this ticket.

### Decision 5: Narrow hook payload before mapping

`useEstructuraRespuestaIdTarea` should avoid passing an untyped payload into the mapper. The implementation should use the existing DTO type when extracting the first payload item, or add a small type guard if needed.

Alternative considered: keep the current unsafe cast.

Rationale: the ticket explicitly requires no `any` and safer DTO access. Removing the unsafe cast improves the hook without changing behavior.

### Decision 6: Use deterministic variant precedence

If the backend sends more than one supported identifier variant in the same DTO item, the mapper will resolve the value in this order:

1. `idRespuestaRadicado`
2. `IdRespuestaRadicado`
3. `ID_RESPUESTA_RADICADO`
4. `id_respuesta_radicado`

Alternative considered: treat multiple variants as an error.

Rationale: the frontend must preserve runtime compatibility with existing backend contracts. Deterministic precedence avoids crashes or inconsistent behavior while preferring the already-normalized camelCase form when available.

## Risks / Trade-offs

- Backend sends multiple variants in the same item -> The mapper uses the documented precedence order and tests cover the chosen behavior.
- Backend sends an unsupported casing variant -> The normalized value remains `undefined`; this protects consumers from undocumented API drift.
- Hook payload remains loosely typed from API response extraction -> Use a DTO-specific narrowing step before mapping to avoid `any`.
- Existing tests may assert exact object shape -> Update expectations only where the optional `idRespuestaRadicado` field is intentionally present or absent.
- Documentation path is deep and specific -> Create the required files during implementation so traceability remains aligned with the ticket.

## Migration Plan

1. Update DTO and normalized model types.
2. Update `mapEstructuraRespuesta` to normalize `idRespuestaRadicado`.
3. Update `useEstructuraRespuestaIdTarea` typing so it passes a typed DTO item to the mapper.
4. Add or update mapper tests for every supported casing variant, precedence, and fallback.
5. Add or update hook tests to verify the normalized field is returned without exposing backend casing.
6. Ensure new or updated tests reference `[SPEC:SCRUMCORE-219]`.
7. Add required technical documentation and metadata.
8. Run focused tests, TypeScript checks, and OpenSpec validation.

Rollback is straightforward: revert the type, mapper, hook typing, tests, and documentation changes. No persisted data, endpoint, or UI state migration is involved.

## Open Questions

- None for the OpenSpec phase. The implementation should inspect the current test layout before choosing exact test filenames.
