# create-cotrato-ag-grid-fase-2 Specification

## Purpose
TBD - created by archiving change scrumcore-30-create-cotrato-ag-grid-fase-2. Update Purpose after archive.
## Requirements
### Requirement: Adaptacion tipada de DynamicUiTableDto a modelo AG Grid
El sistema MUST transformar el contrato backend `DynamicUiTableDto` hacia un modelo intermedio `AppDataTableAgGrid` que exponga `rows`, `columns`, acciones y metadatos necesarios para render, sin propagar el DTO original a la capa visual.

#### Scenario: Transformacion exitosa del contrato
- **WHEN** el modulo recibe un `DynamicUiTableDto` valido desde backend
- **THEN** genera una estructura adaptada para AG Grid lista para consumirse en la UI

#### Scenario: Payload real con PascalCase y metadata
- **WHEN** el backend entrega propiedades como `TableId`, `Columns`, `Rows`, `Pagination`, `Sorting` y `meta`
- **THEN** la adaptacion conserva esa informacion relevante en el modelo interno sin exigir un shape alterno

### Requirement: AppTable desacoplado del backend de contratos
El sistema MUST mantener `AppTable` como componente presentacional y NO MUST requerir conocimiento de contratos HTTP, DTOs backend ni reglas del dominio de contratos.

#### Scenario: Render con datos ya adaptados
- **WHEN** el contenedor entrega `rows` y `columns` adaptados a `AppTable`
- **THEN** la tabla se renderiza sin interpretar `DynamicUiTableDto` ni invocar mapeos adicionales

### Requirement: Mapeo consistente de columnas dinamicas
El sistema MUST traducir la definicion dinamica de columnas del contrato backend hacia configuraciones de columna compatibles con AG Grid, preservando orden, identificadores, visibilidad y metadata de filtro requerida para el render.

#### Scenario: Columnas backend preservan orden
- **WHEN** el contrato backend define varias columnas en un orden especifico
- **THEN** el adapter entrega columnas AG Grid en el mismo orden funcional esperado

#### Scenario: Columnas ocultas no se renderizan
- **WHEN** una columna backend tiene `Visible = false`
- **THEN** la columna no aparece en la salida visible del grid

#### Scenario: Metadata de filtros se preserva
- **WHEN** una columna backend define `FilterType`, `AgGridFilterType` o `FilterOptions`
- **THEN** el adapter conserva esa metadata en el modelo interno

### Requirement: Mapeo de filas desacoplado y estable
El sistema MUST transformar `UiRowDto[]` a `AppGridRow[]` aplanando `Values`, garantizando `id` estable y preservando `Meta` en una propiedad separada.

#### Scenario: Values se aplana sin mezclar Meta
- **WHEN** una fila backend contiene `Values` y `Meta`
- **THEN** `Values` se copia a `data` y `Meta` se conserva aparte

#### Scenario: Rows nulo produce empty state compatible
- **WHEN** `Rows` es `null` o `undefined`
- **THEN** el adapter retorna una coleccion vacia sin lanzar error

### Requirement: Mapeo de acciones anidadas y extensibles
El sistema MUST mapear `UiActionDto` y `UiCellActionDto`, incluyendo el caso real `CellActions[].Action`, preservando metadata completa sin ejecutar comportamiento.

#### Scenario: CellActions con Action anidada
- **WHEN** el backend entrega `CellActions` con `ColumnKey` y una accion anidada en `Action`
- **THEN** el mapper desanida la accion y la asocia a la columna correspondiente

#### Scenario: Behavior y presentation permanecen abiertos
- **WHEN** el backend entrega valores no cerrados para `Behavior` o `Presentation`
- **THEN** el mapper los conserva como strings sin rigidizar el contrato

### Requirement: Cobertura de pruebas del adapter y la integracion
El sistema MUST contar con pruebas automatizadas para validar la transformacion del contrato y la compatibilidad con el payload real del backend sin depender de red real.

#### Scenario: Validacion del adapter
- **WHEN** se ejecutan los tests del adapter con un `DynamicUiTableDto` representativo
- **THEN** las pruebas verifican el mapeo de filas, columnas y metadatos sin depender de red real

#### Scenario: Validacion con payload backend real
- **WHEN** se ejecutan pruebas usando el shape real de `Columns`, `Rows`, `CellActions`, `Pagination` y `Sorting`
- **THEN** la salida final coincide con el modelo interno esperado

