## ADDED Requirements

### Requirement: Backend export en pagina actual habilita Excel/PDF
Cuando un consumidor provee `dataSource.getBackendExportFile`, el componente `AppTableExport` SHALL considerar ejecutables las exportaciones `xlsx` y `pdf` para el modo `currentPage`, ademas de `allMatching`.

#### Scenario: Excel/PDF ejecutable en currentPage con backend export
- **WHEN** `dataSource.getBackendExportFile` esta definido y el usuario intenta exportar en formato `xlsx` o `pdf` con modo `currentPage`
- **THEN** la opcion se muestra habilitada y el flujo de exportacion usa backend export (no el flujo client-side)

#### Scenario: Sin backend export, currentPage mantiene CSV-only
- **WHEN** `dataSource.getBackendExportFile` NO esta definido y el usuario intenta exportar `currentPage`
- **THEN** `csv` se mantiene ejecutable y `xlsx/pdf` se muestran deshabilitados (comportamiento actual)

### Requirement: Selected rows permanece seguro sin soporte backend explicito
El componente `AppTableExport` MUST mantener `selectedRows` como exportacion client-side limitada a `csv`, y MUST NOT invocar backend export con `ExportMode = selectedRows` en la implementacion actual.

#### Scenario: Selected rows no habilita Excel/PDF sin soporte backend
- **WHEN** el usuario abre el menu de exportacion y selecciona el modo `selectedRows`
- **THEN** `xlsx/pdf` se muestran deshabilitados y `csv` solo es ejecutable si existe seleccion

#### Scenario: No se llama backend con selectedRows
- **WHEN** existe `dataSource.getBackendExportFile` y el usuario intenta exportar `selectedRows`
- **THEN** el componente no invoca `getBackendExportFile` para esa combinacion y evita errores funcionales del backend

### Requirement: Labels del menu reflejan combinaciones ejecutables
El menu de `AppTableExport` MUST evitar mostrar "(proximamente)" en un formato cuando exista al menos un modo ejecutable para ese formato.

#### Scenario: Parent label no indica proximamente si existe un modo ejecutable
- **WHEN** un formato (por ejemplo `xlsx`) tiene al menos un modo ejecutable (por ejemplo `currentPage` o `allMatching`)
- **THEN** el label del formato se muestra como "Exportar en <Formato>" sin sufijo "(proximamente)"

