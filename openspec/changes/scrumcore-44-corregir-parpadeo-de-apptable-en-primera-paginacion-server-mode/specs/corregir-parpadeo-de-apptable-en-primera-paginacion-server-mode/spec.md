## ADDED Requirements

### Requirement: La query shared preserva datos durante refetch de paginación server
El sistema SHALL conservar los datos previos mientras se solicita una nueva página en `server mode`, evitando una transición temporal a estado vacío.

#### Scenario: Cambio de página conserva filas previas hasta recibir la nueva respuesta
- **GIVEN** una tabla server con una página ya cargada
- **WHEN** el usuario solicita una nueva página no cacheada
- **THEN** `useDynamicUiTableQuery` mantiene filas y total previos durante el refetch
- **AND** no entrega temporalmente `rows = []` ni `total = 0`

### Requirement: La corrección no rompe el empty state real
El sistema SHALL seguir mostrando empty state real cuando la nueva respuesta no trae filas.

#### Scenario: Respuesta vacía real se respeta
- **WHEN** la nueva consulta server responde sin filas válidas
- **THEN** el resultado final expone estado vacío real
- **AND** no conserva indefinidamente la página anterior

### Requirement: La corrección no rompe la integración del módulo consumidor
El sistema SHALL mantener estable la integración actual de módulos como `GestionCorrespondencia` después del ajuste shared.

#### Scenario: GestionCorrespondencia mantiene query state y render server
- **WHEN** el módulo consume `useDynamicUiTableQuery` a través de su hook
- **THEN** la primera paginación no introduce un flash vacío
- **AND** las pruebas del módulo siguen pasando
