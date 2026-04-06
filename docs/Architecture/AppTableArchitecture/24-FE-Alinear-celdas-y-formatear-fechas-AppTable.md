# Ticket 24 FE

## Titulo

Alinear verticalmente celdas de `AppTable` y formatear fechas dinamicas

## Objetivo

Estandarizar la presentacion visual de celdas en `AppTable` para que el contenido quede centrado verticalmente y los campos de fecha provenientes del contrato dinamico se muestren en un formato legible, sin exponer valores crudos como `2025-04-08T00:00:00`.

## Problema actual

- algunas celdas con contenido de accion o texto corto, como `Publicar`, pueden quedar alineadas en la parte superior de la celda
- la alineacion no debe depender exclusivamente de que la columna venga marcada con `IsActionColumn = true`
- el backend puede enviar columnas de accion usando `RenderType = grid_actions`
- los campos de fecha pueden llegar como string ISO o date-time sin formatear
- valores como `2025-04-08T00:00:00` reducen la legibilidad de la tabla

## Alcance

- ajustar el centrado vertical de celdas en el componente shared `AppTable`
- soportar celdas normales, wrappers internos de AG Grid y celdas de accion
- detectar columnas de accion por:
  - `IsActionColumn`
  - `isActionColumn`
  - `RenderType = grid_actions`
  - `renderType = grid_actions`
  - `Presentation = actions`
  - `presentation = actions`
- aplicar formateo de fechas a columnas dinamicas tipadas como fecha
- mantener compatibilidad con `presentationMode="table"` y `presentationMode="cards"`
- mantener el contrato dinamico desacoplado del modulo consumidor

## No alcance

- no rediseñar `AppTable`
- no cambiar la estructura del DTO backend
- no cambiar la paginacion
- no modificar la semantica de acciones dinamicas
- no implementar nuevos tipos de renderer visual
- no acoplar el ajuste a `GestionCorrespondencia` ni a una pantalla especifica

## Dependencias

- contrato dinamico actual de columnas `DynamicUiTableDto`
- normalizacion existente de `DynamicUiTableDto` hacia `AppGridColumn`
- adapter existente de `AppGridColumn` hacia `ColDef`
- estilos compartidos de `AppTable`

## Archivos frontend esperados

- `src/app/Components/UI/AppTable/AppTable.module.css`
- `src/app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns.ts`
- `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts`
- `src/app/Components/UI/AppTable/renderers/AppTableActionCellRenderer.module.css`
- `src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx`
- `src/app/Components/UI/AppTable/utils/appTableValueFormatters.ts`
- tests de adapters y renderers relacionados

## Reglas de implementacion

- el centrado vertical debe aplicarse en la capa shared de `AppTable`
- el ajuste no debe depender de clases de una pantalla consumidora
- las celdas de AG Grid deben considerar wrappers internos como:
  - `.ag-cell`
  - `.ag-cell-wrapper`
  - `.ag-cell-value`
  - `.ag-react-container`
- las celdas en modo edicion no deben ser forzadas por el centrado base
- las columnas de accion deben recibir una clase estable, por ejemplo `app-table-action-cell`
- las columnas con `RenderType = grid_actions` deben tratarse como columnas de accion aunque el backend no envie `IsActionColumn = true`
- el formateo de fecha debe activarse solo por metadata de columna, no por inspeccion global de cualquier string
- columnas `date` deben mostrar solo fecha
- columnas `datetime` deben conservar hora cuando exista una hora significativa
- si el valor no coincide con un formato de fecha soportado, debe preservarse el valor original
- el modo cards debe reutilizar `valueFormatter` cuando exista para evitar divergencia entre tabla y tarjetas

## Riesgos a evitar

- resolver la alineacion solo para una columna llamada `Publicar`
- depender exclusivamente de `IsActionColumn`
- romper el layout de AG Grid en edicion inline
- perder alineacion horizontal configurada por columna
- formatear strings que no representan fechas
- cambiar el valor real de la fila en vez de solo su presentacion
- duplicar logica de formateo entre tabla y cards

## Pruebas obligatorias

- columna de accion con `IsActionColumn = true` recibe renderer, clase y estilos esperados
- columna con `RenderType = grid_actions` se normaliza como accion aunque no venga `IsActionColumn`
- valor `2025-04-08T00:00:00` en columna `date` se muestra como `08/04/2025`
- valor `2025-04-08T13:45:10` en columna `datetime` se muestra conservando hora
- `presentationMode="cards"` reutiliza formatter cuando la columna lo define
- `AppTable` sigue renderizando tabla sin romper paginacion, loading ni empty state

## Criterios de aceptacion

- el contenido de celdas de `AppTable` queda centrado verticalmente de forma reusable
- celdas como `Publicar` no quedan pegadas a la parte superior
- las columnas de accion se detectan correctamente por flag y por `RenderType`
- las fechas dinamicas no muestran el formato crudo ISO cuando la columna esta tipada como fecha
- la solucion aplica al componente shared y no a un modulo concreto
- las pruebas relacionadas pasan sin regresion funcional
