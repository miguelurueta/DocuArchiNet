# Design: SCRUMCORE-172 (AppEditorPdf - Segmentacion Visual por Area Util)

## Goal

Definir el diseño para que `AppEditorPdf` segmente visualmente por area util (content height) y mantenga estabilidad UX sin alterar el HTML persistido.

## Key Concepts

- `pageHeight`: alto total de pagina (formato/orientacion).
- `pageMargins`: margenes configurables.
- `contentHeight`: `pageHeight - top - bottom`.
- `pageStride`: distancia visual entre inicios de pagina (incluye gap).

## Proposed Behavior

- El calculo de guias, overlays y espaciadores usa `contentHeight` como referencia.
- En `paginationMode="visual"`:
  - Se renderizan hojas visuales y guias.
  - El editor mantiene scroll continuo.
- El engine de edicion sigue siendo `AppEditor` (documento continuo).

## Performance Strategy

- Calculos derivados (`contentHeight`, `pageStride`) en `useMemo`.
- Observacion acotada:
  - ResizeObserver sobre bloques top-level cuando sea estrictamente necesario.
  - debounce para repaginacion automatica.
- Reglas:
  - No repaginar en cada keypress.
  - Priorizar repaginacion inmediata solo en eventos costosos (paste, cambios grandes, imagenes al cargar).

## Stability Strategy

- Preservar seleccion:
  - no reposicionar el cursor durante repaginacion visual.
- Preservar scroll:
  - capturar y restaurar un "anchor" al repaginar.

## Non-Goals

- No insertar marcadores persistidos.
- No cambiar contratos publicos de modulos consumidores.

