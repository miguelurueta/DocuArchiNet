# SCRUMCORE-224 - Implementacion Detallada

## Layout strategy
- `DocumentosWorkbench` incorpora un header local del panel de lista para mostrar el contador.
- Estructura usada: `listPanel` con `grid-template-rows: auto minmax(0,1fr)` para mantener header estable y tabla ocupando el resto.
- El contador no altera el rail ni el visor embebido.

## Scroll strategy
- No se cambia la estrategia de scroll existente de SCRUMCORE-223.
- El contador se renderiza fuera del grid para no interferir con virtualizacion ni header de columnas.

## Sizing strategy
- `listPanel` y `listSurface` preservan `min-height: 0` para evitar overflow accidental.
- La tabla continua en `tableLayoutMode="fill"` y mantiene comportamiento previo.

## Derivacion automatica
- En `useGestionRespuestaDocumentosTable`:
  - `totalDocumentsCount` se calcula por estado derivado.
  - `selectedDocumentsCount` se calcula por cardinalidad de `selectedRowIds`.
- Reglas de total:
  1. `Total`
  2. `TotalRecords`
  3. `rows.length`
- Post-mutacion runtime (`agregar_item`, `eliminar_item`): prioridad runtime (`rows/treeRows` actuales).

## Source of truth
- Lista: `latestRowRef` normalizado por `rowId`.
- Seleccion: callback de `AppTreeTable.onSelectionChanged`.
- Sin `contador++`, `contador--`, ni estado duplicado manual.

## Wiring seleccion
- Se extiende `AppTreeTable` con prop opcional `onSelectionChanged(rowIds)`.
- Internamente convierte `selectedRows` de `AppTable` a `rowIds` de nodos de arbol.
- `DocumentosWorkbench` delega al hook mediante `documentosTable.onSelectionChanged`.

## Compatibilidad AppTreeTable / AppTable
- Cambio backward-compatible:
  - Nueva prop opcional en `AppTreeTable`.
  - Consumidores existentes no cambian.
- `AppTable` no se modifica.

## Responsive behavior
- Conserva `variant="overlay"` para mobile/tablet.
- El contador se mantiene estable en ambos variantes.
- No se alteran reglas de colapso del rail.

## Archivos modificados
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/app/Components/UI/AppTreeTable/types.ts`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
- `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`
