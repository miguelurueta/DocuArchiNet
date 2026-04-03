## ADDED Requirements

### Requirement: AppTable soporta modos explícitos de paginación
El sistema SHALL permitir que `AppTable` opere en modos `none`, `client` y `server` sin romper el comportamiento existente cuando no se informen las nuevas props de paginación.

#### Scenario: Compatibilidad hacia atrás sin paginationMode
- **WHEN** un consumidor renderiza `AppTable` sin `paginationMode`
- **THEN** el componente conserva el comportamiento previo
- **AND** no activa paginación nativa de AG Grid

#### Scenario: none mode renderiza todas las filas recibidas
- **WHEN** `AppTable` recibe `paginationMode = "none"`
- **THEN** el grid no activa paginación nativa
- **AND** renderiza todas las filas recibidas

#### Scenario: client mode activa paginación nativa del grid
- **WHEN** `AppTable` recibe `paginationMode = "client"`
- **THEN** el grid activa `pagination = true`
- **AND** usa `clientPaginationPageSize` o el default `25`

#### Scenario: server mode desactiva la paginación del grid
- **WHEN** `AppTable` recibe `paginationMode = "server"`
- **THEN** el grid mantiene `pagination = false`
- **AND** asume que las filas ya representan la página actual

### Requirement: quickFilterText solo afecta modos locales
El sistema SHALL aplicar `quickFilterText` únicamente en modos locales y no debe alterar resultados en `server mode`.

#### Scenario: quick filter local en client mode
- **WHEN** `AppTable` recibe `quickFilterText` y `paginationMode = "client"`
- **THEN** el grid aplica ese texto como filtro local

#### Scenario: quick filter local en none mode
- **WHEN** `AppTable` recibe `quickFilterText` y `paginationMode = "none"`
- **THEN** el grid aplica ese texto como filtro local

#### Scenario: quick filter ignorado en server mode
- **WHEN** `AppTable` recibe `quickFilterText` y `paginationMode = "server"`
- **THEN** el grid no altera localmente la página renderizada

### Requirement: La configuración base del grid se centraliza sin mezclar backend
El sistema SHALL resolver la configuración base de paginación en `useAgGridBaseConfig` y mantener `AppTable` libre de lógica backend o de query state.

#### Scenario: paginationPageSize solo aplica en client mode
- **WHEN** `clientPaginationPageSize` se informa fuera de `client mode`
- **THEN** el grid ignora ese valor sin romper render ni overlays

#### Scenario: server mode queda controlado por el wrapper externo
- **WHEN** `AppTable` opera en `server mode`
- **THEN** la navegación de páginas queda completamente a cargo del contenedor externo
- **AND** el grid permanece como renderer base
