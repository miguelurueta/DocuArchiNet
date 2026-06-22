## ADDED Requirements

### Requirement: Seleccion multiple en miniaturas
El sistema SHALL permitir seleccionar multiples paginas desde el panel de miniaturas sin perder el comportamiento de pagina activa.

#### Scenario: Click normal selecciona pagina activa
- **WHEN** el usuario hace click normal en una miniatura
- **THEN** la pagina se muestra como pagina activa
- **AND** la seleccion multiple existente no se modifica

#### Scenario: Ctrl click agrega o remueve una pagina
- **WHEN** el usuario hace Ctrl+click o Cmd+click sobre una miniatura
- **THEN** la pagina se marca o desmarca dentro de `selectedPageIds`
- **AND** la pagina tambien queda activa para preview

#### Scenario: Checkbox selecciona pagina
- **WHEN** el usuario activa el checkbox de una miniatura
- **THEN** esa pagina se agrega a `selectedPageIds`

#### Scenario: Contador de seleccion
- **WHEN** existen paginas seleccionadas
- **THEN** el sistema muestra la cantidad seleccionada con el formato `N paginas seleccionadas`
- **AND** muestra un badge contextual `N seleccionadas` en la toolbar unica del preview

### Requirement: Acciones masivas
El sistema SHALL reutilizar los botones existentes de la toolbar cuando exista seleccion multiple.

#### Scenario: Rotar paginas seleccionadas
- **WHEN** el usuario pulsa el boton existente `Rotar derecha`
- **THEN** se invoca `rotatePage(pageId, 90)` para cada pagina seleccionada

#### Scenario: Rotar izquierda paginas seleccionadas
- **WHEN** el usuario pulsa el boton existente `Rotar izquierda`
- **THEN** se invoca `rotatePage(pageId, 270)` para cada pagina seleccionada

#### Scenario: Eliminar paginas seleccionadas
- **WHEN** el usuario pulsa el boton existente `Eliminar pagina`
- **THEN** el sistema solicita confirmacion
- **AND** si se confirma, invoca `removePage(pageId)` para cada pagina seleccionada
- **AND** limpia `selectedPageIds`

#### Scenario: No toolbar secundaria
- **WHEN** existen paginas seleccionadas
- **THEN** no se renderiza una toolbar secundaria
- **AND** no se renderizan botones duplicados `Rotar derecha seleccionadas`, `Rotar izquierda seleccionadas`, `Eliminar paginas seleccionadas` ni `Aplicar crop seleccionadas` en el preview

#### Scenario: Seleccionar todo
- **WHEN** el usuario pulsa `Seleccionar todo`
- **THEN** todas las paginas del documento quedan en `selectedPageIds`

#### Scenario: Deseleccionar todo
- **WHEN** el usuario pulsa `Deseleccionar todo`
- **THEN** `selectedPageIds` queda vacio

### Requirement: Organizador compatible
El sistema SHALL mantener compatibilidad con el organizador de paginas 2x2, 3x3, 4x4, 5x5 y 6x6.

#### Scenario: Organizador reutiliza seleccion central
- **WHEN** el usuario selecciona paginas desde el organizador
- **THEN** se actualiza el mismo `selectedPageIds` usado por miniaturas y toolbar

#### Scenario: Drag and drop preservado
- **WHEN** el usuario reordena paginas mediante drag and drop
- **THEN** el sistema invoca `reorderPages` con el nuevo orden
- **AND** no duplica estado de seleccion por vista

### Requirement: Rendimiento y consistencia
El sistema SHALL evitar duplicidad de indicadores y estados para 10, 50, 100 y 300 paginas.

#### Scenario: Seleccion depurada
- **WHEN** cambian las paginas disponibles despues de eliminar o limpiar
- **THEN** `selectedPageIds` remueve cualquier ID inexistente

#### Scenario: Virtualizacion CSS preservada
- **WHEN** el documento supera el umbral de virtualizacion existente
- **THEN** el panel conserva el atributo de virtualizacion CSS
