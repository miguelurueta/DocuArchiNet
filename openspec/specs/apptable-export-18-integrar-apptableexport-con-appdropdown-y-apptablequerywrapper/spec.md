# apptable-export-18-integrar-apptableexport-con-appdropdown-y-apptablequerywrapper Specification

## Purpose
TBD - created by archiving change scrumcore-54-apptable-export-18-integrar-apptableexport-con-appdropdown-y-apptablequerywrapper. Update Purpose after archive.
## Requirements
### Requirement: AppTableQueryWrapper expone un slot para acciones operativas de tabla
The system SHALL exponer en `AppTableQueryWrapper` un slot dedicado para acciones operativas del dataset, separado de `headerActions`, para integrar capacidades como `AppTableExport` sin acoplar logica de exportacion al wrapper.

#### Scenario: exportacion se monta en el slot de acciones de tabla
- **WHEN** una pantalla compone `AppTableQueryWrapper` con un trigger reusable de exportacion
- **THEN** el trigger se renderiza en el slot de acciones operativas de tabla y no dentro de `headerActions`

#### Scenario: headerActions mantiene su semantica de cabecera
- **WHEN** una pantalla usa `headerActions` junto con acciones operativas de tabla
- **THEN** el wrapper mantiene ambos espacios separados con semantica visual distinta

### Requirement: AppTableQueryWrapper agrupa paginacion y exportacion en la misma banda funcional
The system SHALL renderizar rango visible, page size, navegacion y acciones operativas de tabla dentro del mismo bloque de controles del wrapper.

#### Scenario: desktop muestra exportacion junto a la banda de paginacion
- **WHEN** el wrapper se renderiza en un layout desktop con acciones de tabla
- **THEN** el trigger de exportacion comparte la misma banda visual que el rango visible y la navegacion

#### Scenario: responsive mantiene exportacion dentro del bloque de tabla
- **WHEN** el layout se reacomoda por ancho disponible
- **THEN** la exportacion permanece dentro del mismo bloque funcional de controles de tabla aunque cambie de fila

### Requirement: AppTableExport usa AppDropdown como patron oficial del menu de descarga
The system SHALL mantener `AppDropdown` como componente visual oficial para mostrar formatos y modos disponibles de `AppTableExport` dentro de la integracion con el wrapper.

#### Scenario: el menu refleja solo modos y formatos habilitados
- **WHEN** `AppTableExport` recibe un datasource con capacidades parciales y una lista de formatos soportados
- **THEN** el dropdown muestra solo las opciones realmente disponibles para esa tabla

#### Scenario: el trigger integrado sigue siendo reusable
- **WHEN** otra pantalla monta `AppTableExport` dentro del slot operativo del wrapper
- **THEN** la integracion reutiliza el mismo componente sin wiring especifico de modulo

### Requirement: exportLoading no altera el estado visual de carga de la tabla
The system SHALL mantener `exportLoading` aislado del `loading` de datos de `AppTableQueryWrapper` y MUST NOT activar skeleton ni overlays de tabla durante la descarga.

#### Scenario: descarga en curso mantiene tabla visible
- **WHEN** `AppTableExport` inicia una descarga local desde la banda de controles
- **THEN** el contenido actual de la tabla permanece visible

#### Scenario: exportLoading no reemplaza loading de tabla
- **WHEN** una pantalla tiene `exportLoading` activo y `loading` de tabla inactivo
- **THEN** el wrapper no activa estados visuales equivalentes a recarga de datos

### Requirement: AppTableExport usa AppDropdown como patron oficial de menu de descarga
The system SHALL renderizar las opciones de exportacion de `AppTableExport` mediante `AppDropdown`, agrupando las acciones por formato y mostrando solo los modos realmente disponibles para el datasource activo.

#### Scenario: Menu de exportacion refleja capacidades reales del datasource
- **WHEN** `AppTableExport` recibe un datasource y una lista de formatos habilitados
- **THEN** el dropdown muestra un submenu por cada formato visible y solo incluye modos de exportacion soportados por ese datasource

#### Scenario: Formatos no soportados no se presentan como accion ejecutable
- **WHEN** `AppTableExport` recibe un formato aun no implementado localmente
- **THEN** el dropdown mantiene la opcion visible como no ejecutable y no dispara una descarga local para ese formato

### Requirement: AppTableQueryWrapper expone una banda operativa para acciones de tabla
The system SHALL permitir montar acciones operativas de tabla en `paginationActions` de `AppTableQueryWrapper`, manteniendolas en la misma banda visual que el rango visible, el page size y la navegacion de pagina.

#### Scenario: Exportacion convive con la paginacion en la misma banda
- **WHEN** una vista renderiza `AppTableQueryWrapper` con `paginationActions` que incluye `AppTableExport`
- **THEN** el trigger de exportacion aparece en la misma zona funcional que los controles de paginacion y no en un toolbar separado de la tabla

#### Scenario: HeaderActions conserva un rol distinto al de acciones operativas
- **WHEN** la vista necesita acciones de cabecera y acciones operativas de tabla en la misma pantalla
- **THEN** `headerActions` permanece reservado para acciones del bloque superior y `paginationActions` absorbe acciones como exportacion que dependen del contexto de la tabla

### Requirement: La exportacion mantiene un estado de carga no destructivo para la tabla visible
The system SHALL expresar la descarga en curso mediante un estado de exportacion propio y MUST NOT reutilizar el `loading` de datos de la tabla para bloquear o sustituir el contenido actualmente renderizado.

#### Scenario: Descarga en curso no activa skeleton ni overlay de tabla
- **WHEN** el usuario inicia una exportacion desde `AppTableExport`
- **THEN** la tabla permanece visible con sus datos actuales y el sistema no activa skeletons ni overlays de recarga de filas por esa operacion

#### Scenario: Estado de exportacion bloquea solo la accion de descarga
- **WHEN** `AppTableExport` entra en estado de exportacion
- **THEN** el trigger y las opciones del dropdown asociadas a la descarga quedan temporalmente deshabilitadas sin convertir toda la tabla en estado de carga

### Requirement: La composicion responsive mantiene la descarga dentro del contexto visual de la tabla
The system SHALL conservar `AppTableExport` dentro del mismo bloque responsive de controles de `AppTableQueryWrapper`, incluso cuando el layout se reorganiza en multiples filas.

#### Scenario: Responsive reacomoda controles sin separar exportacion del wrapper
- **WHEN** el viewport obliga a reacomodar la banda de controles de `AppTableQueryWrapper`
- **THEN** la accion de exportacion sigue formando parte del mismo bloque visual que rango, page size y navegacion, aunque cambie de linea

#### Scenario: Integracion reusable no depende de una pantalla concreta
- **WHEN** otro modulo reutiliza `AppTableQueryWrapper` y `AppTableExport` con el mismo patron de composicion
- **THEN** la ubicacion y semantica visual de la exportacion se preservan sin wiring especifico de una vista particular

