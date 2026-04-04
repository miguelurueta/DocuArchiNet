## ADDED Requirements

### Requirement: GestionCorrespondencia adopta el query state reusable
El sistema SHALL migrar `GestionCorrespondencia` para que `AppTableQueryState` sea la única fuente de verdad de búsqueda, paginación y sort del módulo.

#### Scenario: Hook del módulo expone queryState y onQueryChange
- **WHEN** un consumidor usa `useGestionCorrespondenciaTable`
- **THEN** el hook expone `queryState` y `onQueryChange`
- **AND** no mantiene estados paralelos de búsqueda, paginación o sort

### Requirement: La pantalla compone el wrapper reusable y AppTable en server mode
El sistema SHALL renderizar `GestionCorrespondencia` usando `AppTableQueryWrapper` como capa visual de controles y `AppTable` como renderer final con `paginationMode = "server"`.

#### Scenario: Wrapper reemplaza la barra manual
- **WHEN** `GestionCorrespondencia` renderiza la tabla principal
- **THEN** usa `AppTableQueryWrapper` para búsqueda, refresh y paginación externa
- **AND** no renderiza otra barra paralela con los mismos controles

#### Scenario: AppTable opera como renderer de la página actual
- **WHEN** `GestionCorrespondencia` renderiza `AppTable`
- **THEN** la tabla usa `paginationMode = "server"`
- **AND** las filas visibles corresponden a la página actual entregada por backend

### Requirement: La migración preserva integración dinámica existente
El sistema SHALL mantener intactas las capacidades dinámicas ya soportadas por la tabla del módulo.

#### Scenario: Acciones dinámicas y menús se preservan
- **WHEN** `GestionCorrespondencia` migra al nuevo wrapper
- **THEN** `MenuActions`, dropdowns y acciones de columna siguen funcionando

#### Scenario: Columnas fijas se preservan
- **WHEN** el backend entrega metadata de columnas fijas
- **THEN** `GestionCorrespondencia` sigue renderizando `Pinned/LockPinned` correctamente

### Requirement: La migración no rompe la navegación del módulo
El sistema SHALL preservar el patrón actual de ruta, drawer y subruta `respuesta`.

#### Scenario: RoutePage y subruta respuesta siguen operativas
- **WHEN** `GestionCorrespondencia` adopta la infraestructura reusable
- **THEN** `GestionCorrespondenciaRoutePage` mantiene su responsabilidad de ruta
- **AND** la subruta `respuesta` y el patrón `Outlet + Drawer` no se rompen
