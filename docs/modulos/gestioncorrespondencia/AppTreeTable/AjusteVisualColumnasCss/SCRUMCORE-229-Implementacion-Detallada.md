# SCRUMCORE-229 — Implementación Detallada

## Ubicación (scope)
El ajuste visual está **scopeado al Workbench** (no global):

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

> Nota: Se agregaron tooltips de header a nivel `AppTreeTable` para asegurar que existan en cualquier configuración de columnas. Ver sección “Tooltips”.

## Cambios de comportamiento (click selecciona fila completa)
En `DocumentosWorkbench` se habilitó la selección por click de fila:

- `suppressRowClickSelection={false}`

Esto permite que AG Grid marque la fila con `aria-selected="true"` y el CSS de “selected row” pinte **toda la fila** de forma uniforme.

Archivo:
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

## Header del rail (botón colapsar junto a "Documentos (N)")
Se movió el botón de colapsar al header del listado para que quede a la derecha del contador:

- Se añadió un `AppButton` en el `header` del listado.
- Se ocultó el header interno de `AppCollapseRail` para maximizar el alto disponible del listado.

Archivos:
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`

## Estilos AG Grid (Quartz) — CSS-only, scoped
Todos los selectores de AG Grid se aplican **solo** dentro del contenedor:

- `.listSurface :global(.ag-theme-quartz ...)`

### Variables AG Grid usadas
En `.listSurface :global(.ag-theme-quartz)` se definieron variables para suavizar el tema:

- `--ag-border-color`
- `--ag-row-border-color`
- `--ag-odd-row-background-color`
- `--ag-background-color`
- `--ag-header-background-color`
- `--ag-header-foreground-color`
- `--ag-header-height`
- `--ag-row-height`
- `--ag-font-size`
- `--ag-row-hover-color`

### “Sin líneas verticales” (column separators)
Se deshabilitaron bordes/separadores verticales y se dejó únicamente el separador horizontal por fila:

- `--ag-borders: none`
- `--ag-borders-secondary: none`
- `--ag-header-column-separator-display: none`
- En `.ag-header-cell` y `.ag-cell`:
  - `border-left: 0`
  - `border-right: 0`
  - `box-shadow: none`
- Se ocultaron pseudo-elementos separadores (por si Quartz los usa):
  - `.ag-header-cell::after { display: none; }`
  - `.ag-cell::after { display: none; }`

Separador horizontal **solo de filas**:
- `.ag-row { border-bottom: 1px solid rgba(...); }`

Archivo:
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`

### Header como “título” (texto más notable)
Se ajustó el texto del header para que se perciba como título enterprise:

- `.ag-header-cell-label`:
  - `font-weight: 800`
  - `text-transform: uppercase`
  - `letter-spacing: 0.04em`
- `.ag-header-cell-text`:
  - `color: #0b1220`
  - `font-size: 13px`

Archivo:
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`

### Hover
Se usa `--ag-row-hover-color` (sutil) para evitar flashes y mantener scroll suave.

### Selected row (un solo color uniforme)
La fila seleccionada se pinta de manera uniforme:

- `.ag-row[aria-selected="true"] .ag-cell { background-color: rgba(15, 23, 42, 0.035); }`

Sin indicadores/bordes adicionales en columnas específicas.

### Focus visible (accesibilidad)
Se mantiene ring de enfoque visible:

- `.ag-cell-focus` y `.ag-cell:focus-within` con `outline` y `border-radius`.

### Botón de acciones (ellipsis) minimalista
Sin tocar renderers, solo CSS:

- `.app-table-action-cell .ant-btn` sin borde/ fondo
- hover/active con background sutil

## Tooltips en headers (AppTreeTable)
Para garantizar tooltips en headers sin depender de cada configuración de columnas, se añadió:

- `headerTooltip` por defecto en las columnas (si no venía definido).
- En columnas inferidas (`generated`) se setea `headerTooltip: column`.

Archivo:
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`

## Qué NO se tocó
- No se modificó `src/app/Components/UI/AppTable/**`.
- No se cambiaron contratos de datos ni lógica de carga.
- No se cambiaron widths/minWidths/flex desde `DocumentosWorkbench` para columnas.

## Lista exacta de selectores (scoped)
Todos bajo `.listSurface :global(.ag-theme-quartz ...)`:

- `.ag-header`
- `.ag-header-cell`
- `.ag-header-cell::after`
- `.ag-header-cell-label`
- `.ag-header-cell-text`
- `.ag-row`
- `.ag-cell`
- `.ag-cell::after`
- `.ag-row[aria-selected="true"] .ag-cell`
- `.ag-cell-focus`
- `.ag-cell:focus-within`
- `.app-table-action-cell`
- `.app-table-action-cell .ant-btn`
- `.app-table-action-cell .ant-btn:hover`
- `.app-table-action-cell .ant-btn:active`

