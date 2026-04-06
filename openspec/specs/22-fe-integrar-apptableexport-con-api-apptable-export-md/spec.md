# 22-fe-integrar-apptableexport-con-api-apptable-export-md Specification

## Purpose
Formalizar la integracion backend real de `AppTableExport` con `/api/AppTable/export`, manteniendo el componente shared desacoplado del modulo y validando el primer consumo end-to-end en `GestionCorrespondencia`.
## Requirements
### Requirement: AppTableExport soporta exportación backend real mediante estrategia declarativa
El sistema SHALL permitir que `AppTableExport` ejecute exportaciones mediante un proveedor backend reusable, sin acoplar el componente shared a un endpoint o módulo específico.

#### Scenario: El datasource declara capacidad backend de exportación
- **WHEN** el consumidor provee una capacidad reusable de exportación server-side
- **THEN** `AppTableExport` MUST enrutar la descarga por esa estrategia en lugar de asumir generación local del archivo

#### Scenario: El componente shared permanece desacoplado
- **WHEN** se integra un endpoint backend real de exportación
- **THEN** `AppTableExport` MUST seguir sin conocer directamente `/api/AppTable/export` ni `workflowInboxgestion`

### Requirement: La exportación backend reutiliza query state y contrato operativo de la tabla
El sistema SHALL construir la exportación server-side usando la misma semántica de consulta activa de la tabla, incluyendo filtros, búsqueda, ordenamiento y metadata contextual del reporte.

#### Scenario: allMatching conserva la consulta activa
- **WHEN** el usuario ejecuta `allMatching` en una tabla server-side
- **THEN** la exportación backend MUST recibir `search`, `searchType`, `structuredFilters`, `sortField` y `sortDir` coherentes con la tabla visible

#### Scenario: El request backend recibe formato y modo de exportación
- **WHEN** el usuario selecciona un formato y un alcance desde `AppDropdown`
- **THEN** el datasource backend MUST traducir `AppTableExportFormat` y `AppTableExportMode` al contrato de la API de exportación

#### Scenario: La metadata del reporte llega al flujo backend
- **WHEN** la exportación se resuelve por API
- **THEN** el backend MUST recibir al menos la metadata necesaria para construir el archivo final con contexto de reporte

### Requirement: xlsx y pdf se resuelven por backend cuando existe capacidad server-side
El sistema SHALL preferir exportación backend para formatos ejecutivos como `xlsx` y `pdf` cuando el datasource actual exponga una capacidad real de archivo server-side.

#### Scenario: Formatos ejecutivos visibles solo con soporte real
- **WHEN** el datasource actual no soporta exportación backend real
- **THEN** `xlsx` y `pdf` MUST permanecer ocultos o no ejecutables para evitar promesas falsas en la UI

#### Scenario: Formatos ejecutivos usan archivo final server-side
- **WHEN** el usuario dispara una exportación `xlsx` o `pdf` en un datasource con soporte backend
- **THEN** `AppTableExport` MUST descargar el archivo final devuelto por la API y MUST NOT intentar serializarlo localmente

### Requirement: La integración backend conserva la UX no destructiva de la tabla
El sistema SHALL mantener el mismo patrón visual no destructivo durante exportaciones backend, conservando visible la tabla y aislando el loading al trigger de exportación y sus controles.

#### Scenario: Exportación backend en curso
- **WHEN** una exportación backend permanece pendiente
- **THEN** la tabla MUST seguir visible y el menú de exportación MUST reflejar `exportLoading`

#### Scenario: Error en exportación backend
- **WHEN** la operación backend falla o rechaza la descarga
- **THEN** `AppTableExport` MUST restaurar el estado interactivo del trigger y MUST NOT dejar el flujo bloqueado permanentemente

### Requirement: GestionCorrespondencia valida el primer consumo real de exportación backend
El sistema SHALL usar `GestionCorrespondencia` como primer consumidor real de la integración backend de `AppTableExport`, para validar la arquitectura reusable end-to-end.

#### Scenario: El módulo consumidor declara la capacidad backend
- **WHEN** `GestionCorrespondencia` configura su exportación de tabla
- **THEN** MUST proveer la capacidad backend necesaria sin duplicar la lógica shared de `AppTableExport`

#### Scenario: La integración real queda cubierta por pruebas
- **WHEN** se ejecutan las pruebas del reusable y del módulo consumidor
- **THEN** MUST validarse que la exportación backend usa el contrato esperado, preserva la UX y no regresa a serialización local para formatos server-side
