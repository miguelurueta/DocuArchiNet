## ADDED Requirements

### Requirement: Normalized idRespuestaRadicado in structure-by-task flow
The Gestion Correspondencia structure-by-task flow SHALL support typed normalization of `idRespuestaRadicado` from backend DTOs into a single frontend field without exposing backend casing variants to hooks or UI consumers.

#### Scenario: Normalize camelCase backend id
- **WHEN** the structure response DTO includes `idRespuestaRadicado`
- **THEN** the normalized structure item exposes `idRespuestaRadicado` with the same string or number value

#### Scenario: Normalize PascalCase backend id
- **WHEN** the structure response DTO includes `IdRespuestaRadicado`
- **THEN** the normalized structure item exposes `idRespuestaRadicado` with the same string or number value

#### Scenario: Normalize uppercase snake backend id
- **WHEN** the structure response DTO includes `ID_RESPUESTA_RADICADO`
- **THEN** the normalized structure item exposes `idRespuestaRadicado` with the same string or number value

#### Scenario: Normalize snake_case backend id
- **WHEN** the structure response DTO includes `id_respuesta_radicado`
- **THEN** the normalized structure item exposes `idRespuestaRadicado` with the same string or number value

#### Scenario: Resolve multiple backend id variants deterministically
- **WHEN** the structure response DTO includes more than one supported `idRespuestaRadicado` variant
- **THEN** the mapper uses the first available value in this order: `idRespuestaRadicado`, `IdRespuestaRadicado`, `ID_RESPUESTA_RADICADO`, `id_respuesta_radicado`

#### Scenario: Fallback when backend id is absent
- **WHEN** the structure response DTO does not include any supported `idRespuestaRadicado` variant
- **THEN** the normalized structure item exposes `idRespuestaRadicado` as `undefined`
- **AND** it does not use `0`, an empty string, or `NaN`

#### Scenario: Preserve existing normalized fields
- **WHEN** the structure response DTO includes existing fields such as `Radicado`, `Destinatario`, and `TramiteDocumento`
- **THEN** the mapper preserves the current normalized output for those fields
- **AND** adding `idRespuestaRadicado` does not change their values

#### Scenario: Hook consumers use normalized casing only
- **WHEN** `useEstructuraRespuestaIdTarea` returns structure data
- **THEN** consumers can access `idRespuestaRadicado` on the normalized frontend model
- **AND** consumers do not need to inspect backend casing variants
