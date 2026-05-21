# SCRUMCORE-216 - Implementacion Detallada

## 1. Resumen
`AppTreeTable` se convierte en wrapper de `AppTable` y delega el engine tabular. `AppTreeTable` conserva expansion, flattening e indentacion.

Fuera de alcance:
- Contratos backend-driven nuevos.
- Reemplazo de `AppTable`.

## 2. Arbol de carpetas impactadas
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx` (refactor)
- `src/app/Components/UI/AppTreeTable/AppTreeTable.module.css` (render label/toggle)
- `src/app/Components/UI/AppTreeTable/adapters/*` (nuevo)
- `src/app/Components/UI/AppTreeTable/hooks/*` (nuevo)
- `src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx` (ajustes + mock de `AppTable`)

## 3. Implementacion por capas

### Components
- `AppTreeTable.tsx`: orquesta load/loadChildren, estados legacy, y renderiza `AppTable`.

### Hooks
- `useTreeExpansionState`: mantiene `expandedIds` estable y `toggleExpanded`.
- `useTreeVisibleRows`: memoiza `flattenTree(rows, expandedIds)`.

### Adapters
- `flattenTree`: transforma jerarquia en lista lineal con metadata (level/expanded/hasChildren).
- `resolveTreeIndentation`: calcula indentacion por level.
- `mapTreeRowsToAppTableRows`: mapea filas lineales a filas compatibles con `AppTable` con metadata `__tree`.

### Types
- Publico: `AppTreeTableRow` (sin breaking changes).
- Interno: `TreeTableRow` (solo adapters/hook).

## 4. Flujo end-to-end
1. `load()` -> `state.ready(rows)`
2. `flattenTree` + `mapTreeRowsToAppTableRows`
3. `AppTable` renderiza y dispara `onRowClicked`/acciones de celdas
4. Expand/collapse actualiza `expandedIds`, y opcionalmente `loadChildren` si aplica.

## 5. Trazabilidad tecnica
El comportamiento jerarquico se mantiene exclusivamente en `AppTreeTable` (hooks/adapters).

## 6. Pruebas y validacion
Ver `SCRUMCORE-216-Pruebas.md`.

## 7. Deuda tecnica
- Evaluar si `AppTreeTable` debe exponer soporte para acciones de fila/columna en el futuro (sin scope en este ticket).

## 8. Glosario tecnico
- Tree -> Table: adaptacion de jerarquia a filas lineales para motor tabular.

