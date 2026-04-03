## ADDED Requirements

### Requirement: Dynamic UI columns can declare pinned placement
The system SHALL allow backend-provided dynamic columns to declare pinned placement metadata so they can render fixed to the left or right in AG Grid through the shared `AppTable` pipeline.

#### Scenario: Preserve `Pinned` from DTO to AppGridColumn
- **WHEN** a dynamic backend column includes `Pinned = "left"` or `Pinned = "right"`
- **THEN** the shared normalization pipeline MUST preserve that value in `AppGridColumn`
- **AND** the metadata MUST remain available for the final adapter to `ColDef`

### Requirement: Dynamic UI columns can preserve lockPinned semantics
The system SHALL preserve `LockPinned` metadata from the dynamic backend column contract to the final AG Grid column definition.

#### Scenario: Preserve `LockPinned`
- **WHEN** a dynamic backend column includes `LockPinned = true`
- **THEN** the shared pipeline MUST preserve that value
- **AND** the resulting `ColDef` MUST include `lockPinned = true`

### Requirement: AppGridColumn pinning maps to AG Grid ColDef without changing AppTable API
The system SHALL map shared dynamic pinning metadata to AG Grid native `ColDef` fields without introducing a new public API in `AppTable`.

#### Scenario: Map pinned metadata to ColDef
- **WHEN** a shared `AppGridColumn` includes `pinned` and/or `lockPinned`
- **THEN** `appGridToAppTableColumns.ts` MUST map them to `ColDef.pinned` and `ColDef.lockPinned`
- **AND** `AppTable` MUST continue consuming `columns: ColDef<T>[]` without API changes

### Requirement: Columns without pinning metadata remain unchanged
The system SHALL preserve current behavior for dynamic columns that do not include pinning metadata.

#### Scenario: Column without `Pinned`
- **WHEN** a dynamic backend column omits `Pinned` and `LockPinned`
- **THEN** no pinning MUST be applied by default
- **AND** existing table behavior MUST remain unchanged

### Requirement: Manual AppTable column pinning remains compatible
The system SHALL preserve compatibility with current manual uses of `AppTable` that already pass AG Grid `ColDef` objects with native pinning configuration.

#### Scenario: Existing manual ColDef with pinned
- **WHEN** a consumer provides a manual `ColDef` with `pinned`
- **THEN** the feature MUST continue working as before
- **AND** the dynamic pinning implementation MUST NOT alter that manual path

### Requirement: Optional action-column pinning must be explicit if implemented
The system MAY support a reusable convention to pin action columns to the right, but that convention MUST be explicit, documented and covered by tests if adopted.

#### Scenario: Explicit action-column pinning convention
- **WHEN** the shared adapters implement a convention for `isActionColumn`
- **THEN** the convention MUST be applied consistently
- **AND** it MUST be documented and tested
