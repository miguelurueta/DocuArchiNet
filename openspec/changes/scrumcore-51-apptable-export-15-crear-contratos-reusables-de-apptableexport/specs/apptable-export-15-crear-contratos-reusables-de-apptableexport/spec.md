## ADDED Requirements

### Requirement: AppTableExport define modos de exportacion reutilizables

El sistema MUST definir un contrato reusable para expresar el alcance de exportacion de una tabla sin acoplarlo a una pantalla o modulo concreto.

#### Scenario: modos de exportacion disponibles
- **WHEN** una implementacion de exportacion de `AppTable` defina su alcance operativo
- **THEN** el contrato MUST distinguir explicitamente `currentPage`, `selectedRows`, `allLoaded` y `allMatching`

#### Scenario: semantica diferenciada entre datos locales y totales
- **WHEN** una pantalla use datos paginados server-side
- **THEN** el contrato MUST permitir distinguir entre datos visibles localmente y todos los resultados de la consulta

### Requirement: AppTableExport define un datasource desacoplado del dominio

El sistema MUST definir un datasource reusable que permita obtener filas para exportacion sin exponer detalles de un modulo, pantalla o endpoint concreto.

#### Scenario: exportacion local desde filas visibles
- **WHEN** una pantalla necesite exportar pagina actual o seleccionados
- **THEN** el contrato MUST permitir resolver esos datos desde funciones locales del datasource

#### Scenario: exportacion total con estrategia adicional
- **WHEN** una pantalla necesite exportar `allMatching`
- **THEN** el contrato MUST permitir una estrategia asincrona separada para obtener todos los resultados aplicables

### Requirement: AppTableExport define formatos de salida soportados

El sistema MUST formalizar un contrato reusable para los formatos de archivo soportados por el flujo de exportacion.

#### Scenario: formato de exportacion declarado
- **WHEN** una pantalla configure una accion de exportacion
- **THEN** el contrato MUST expresar el formato mediante un tipo reusable y consistente

#### Scenario: compatibilidad entre pantallas
- **WHEN** multiples pantallas consuman el sistema de exportacion
- **THEN** el contrato MUST evitar que cada una invente nombres o representaciones distintas para los mismos formatos

### Requirement: AppTableExport define metadata institucional del reporte

El sistema MUST definir metadata reusable para construir un encabezado ejecutivo de reporte sin dejar esa responsabilidad hardcodeada por pantalla.

#### Scenario: metadata minima del reporte
- **WHEN** una exportacion requiera encabezado institucional
- **THEN** el contrato MUST incluir nombre del reporte, usuario generador, modulo, tipo de reporte, fecha y hora, numero de filas y descripcion

#### Scenario: referencia al asset corporativo
- **WHEN** una exportacion requiera branding institucional
- **THEN** el contrato MUST incluir una referencia a un asset corporativo controlado por el repositorio

### Requirement: El asset corporativo del reporte debe resolverse desde el repositorio

El sistema MUST expresar el logo institucional como un asset del repositorio y no como una URL externa del reporte final.

#### Scenario: origen estable del logo
- **WHEN** el flujo de exportacion construya metadata de branding
- **THEN** el contrato MUST referenciar un asset versionado dentro del repositorio

#### Scenario: reporte final sin dependencia de URL externa
- **WHEN** se genere un archivo final de reporte
- **THEN** el contrato MUST asumir que la imagen sera embebida o insertada en el archivo
- **AND** MUST NOT depender de una URL externa para representar el logo

### Requirement: El contrato de exportacion no debe acoplarse a runtime de AppTable

El sistema MUST mantener separados los contratos de exportacion respecto al renderer base de tabla.

#### Scenario: separacion de responsabilidades
- **WHEN** se definan tipos de exportacion reusable
- **THEN** esos contratos MUST vivir fuera del contrato principal de rendering de `AppTable`

#### Scenario: compatibilidad con fases posteriores
- **WHEN** fases posteriores implementen UI o backend de exportacion
- **THEN** esas fases MUST poder consumir el contrato base sin redefinir su semantica
