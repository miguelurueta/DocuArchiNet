# SCRUMCORE-216 - Design

## Objetivo de diseno
Implementar `AppTreeTable` como wrapper/adaptador reusable sobre `AppTable`, preservando compatibilidad con consumidores actuales y evitando duplicacion del engine tabular.

## Principios / Restricciones
- `AppTable` conserva el engine tecnico: render, integracion con grid, eventos, acciones, estados base, accesibilidad.
- `AppTreeTable` solo aporta: flattening, expansion, indentacion, filas visibles, estados legacy.
- No introducir breaking changes en props ni comportamiento observable para consumidores.
- No acoplar `AppTreeTable` a `GestionCorrespondencia`.
- TypeScript estricto: no `any`.
- No crear contratos nuevos backend-driven en este ticket (solo preparar puntos de extension).

## Arquitectura propuesta

### Componentes
- `AppTreeTable.tsx`
  - Orquesta `rows`/`load`/`loadChildren`, estados legacy y adaptacion.
  - Renderiza internamente `AppTable` con columnas configuradas para arbol (columna label + affordance expand/collapse).

### Hooks (nuevos)
- `useTreeExpansionState()`: mantiene `expandedIds` estable y operaciones expand/collapse.
- `useTreeVisibleRows()`: calcula filas visibles en funcion de expansion + flattening memoizado.

### Adapters / Helpers (nuevos)
- `flattenTree()`: convierte `AppTreeTableRow[]` a una lista lineal con `level`, `parentId`, `hasChildren`.
- `mapTreeRowsToAppTableRows()`: adapta filas lineales a filas consumibles por `AppTable` (incluye render de label con indentacion + affordance).
- `resolveTreeIndentation()`: calcula padding/indent consistente por `level`.

### Types
- Mantener `AppTreeTableRow` publico.
- Interno: `TreeTableRow` con metadatos (`level`, `expanded`, `hasChildren`, `originalNode`, etc.).

## Estructura esperada (archivos)
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTreeTable/types.ts`
- `src/app/Components/UI/AppTreeTable/index.ts`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.module.css`
- `src/app/Components/UI/AppTreeTable/hooks/useTreeExpansionState.ts`
- `src/app/Components/UI/AppTreeTable/hooks/useTreeVisibleRows.ts`
- `src/app/Components/UI/AppTreeTable/adapters/flattenTree.ts`
- `src/app/Components/UI/AppTreeTable/adapters/mapTreeRowsToAppTableRows.ts`
- `src/app/Components/UI/AppTreeTable/adapters/resolveTreeIndentation.ts`

## Flujo principal
1. `load()` obtiene raiz.
2. Se flatea y se computan visibles segun `expanded`.
3. Se mapea a filas `AppTable`.
4. Expand/collapse actualiza `expanded` y recalcula visibles (sin recomputar innecesario).
5. `onSelectRow` se mantiene: seleccion delegada desde `AppTable` hacia `AppTreeTable`.

## Compatibilidad futura (no scope)
- Hooks/adapters listos para incorporar metadata y row actions backend-driven sin cambiar API publica.
- No se implementa lazy loading backend-driven ni contratos nuevos en este ticket.

## Riesgos y mitigaciones
- Re-render masivo al expandir: memoizar flattening y visibles; callbacks estables.
- Perdida de UX existente: mantener estados/mensajes legacy y pruebas de regresion.
- Divergencia con `AppTable`: usar `AppTable` como unico engine de render.

## Criterios de aceptacion
- `AppTreeTable` usa `AppTable` internamente como engine.
- No se cambian props ni se exigen cambios a consumidores actuales.
- Expand/collapse, seleccion, y estados legacy (loading/empty/error/retry) siguen funcionando.
- Pruebas unitarias (adapters/hooks) + integracion (`AppTreeTable` y `DocumentosWorkbench`) cubren el refactor sin regresiones.

