## ADDED Requirements

### Requirement: Header de listado permanece visible durante scroll en DocumentosWorkbench
El sistema SHALL mantener visible el header de columnas del listado de documentos mientras el usuario se desplaza por una lista larga en `DocumentosWorkbench`.

#### Scenario: Scroll vertical en lista extensa
- **WHEN** el usuario desplaza verticalmente el listado con multiples filas
- **THEN** los titulos de columnas permanecen visibles en pantalla

#### Scenario: Alcance local del comportamiento
- **WHEN** se aplica la estrategia de header persistente
- **THEN** el ajuste afecta solo la implementacion de `AppTreeTable` en `DocumentosWorkbench` y no altera otras tablas del sistema

### Requirement: Separacion entre documento activo y seleccion multiple
El sistema SHALL preservar la separacion funcional entre el documento activo visualizado y la seleccion multiple por checkbox.

#### Scenario: Documento activo por click
- **WHEN** el usuario hace click en una fila/celda de documento
- **THEN** se establece un unico documento activo para visualizacion

#### Scenario: Seleccion masiva por checkbox
- **WHEN** el usuario marca uno o mas checkboxes
- **THEN** la seleccion multiple se conserva para operaciones masivas sin reemplazar automaticamente el documento activo

### Requirement: No regresion en acciones de fila
El sistema MUST mantener disponible y funcional la columna de acciones por fila tras la adopcion del header persistente.

#### Scenario: Disparo de accion por fila
- **WHEN** el usuario ejecuta una accion desde el menu de fila
- **THEN** el flujo de `onActionTriggered` se ejecuta sin regresiones funcionales
