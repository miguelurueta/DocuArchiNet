## ADDED Requirements

### Requirement: Contador documental total contextual
El sistema SHALL mostrar en `DocumentosWorkbench` un contador total de documentos consistente con el estado actual del listado.

#### Scenario: Conteo inicial con total backend disponible
- **WHEN** la carga del listado retorna `Total` o `TotalRecords`
- **THEN** el contador inicial usa ese valor como referencia de total

#### Scenario: Fallback cuando backend no entrega total
- **WHEN** la carga del listado no entrega `Total` ni `TotalRecords`
- **THEN** el contador usa `rows.length` como fallback

#### Scenario: Lista vacia
- **WHEN** no existen filas en el listado
- **THEN** el contador muestra `Documentos (0)`

### Requirement: Contador de seleccionados derivado automaticamente
El sistema SHALL mostrar el total de documentos seleccionados sin lógica manual mutable.

#### Scenario: Seleccion multiple por checkbox
- **WHEN** el usuario selecciona o deselecciona filas
- **THEN** `selectedDocumentsCount` se actualiza automaticamente desde la seleccion actual

#### Scenario: Documento activo independiente
- **WHEN** el usuario cambia el documento activo por click
- **THEN** el contador de seleccionados no se altera por ese evento

### Requirement: Sincronizacion automatica post-mutacion runtime
El sistema SHALL recalcular los contadores automaticamente cuando cambia el estado normalizado de lista.

#### Scenario: Agregar item
- **WHEN** una mutacion runtime incorpora nuevas filas al estado actual
- **THEN** el contador total se recalcula automaticamente desde `rows/treeRows` actuales

#### Scenario: Eliminar item
- **WHEN** una mutacion runtime remueve filas del estado actual
- **THEN** el contador total se recalcula automaticamente desde `rows/treeRows` actuales

#### Scenario: Recarga de lista por acciones
- **WHEN** una accion dispara recarga de la lista
- **THEN** los contadores total y seleccionados quedan sincronizados con el dataset actualizado

### Requirement: No-regresion y alcance local
El sistema MUST implementar contadores sin impactar comportamiento global ni contratos existentes.

#### Scenario: Sin impacto global en AppTable/AppTreeTable
- **WHEN** otros modulos consumen `AppTable`/`AppTreeTable`
- **THEN** no cambia su comportamiento por esta implementacion

#### Scenario: Integridad funcional existente
- **WHEN** se utiliza el flujo documental actual
- **THEN** se preservan documento activo, seleccion multiple, Dynamic UI y acciones de fila
