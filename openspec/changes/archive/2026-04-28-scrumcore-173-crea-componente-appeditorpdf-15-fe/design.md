# Design: SCRUMCORE-173 (AppEditorPdf - Hardening Multi-hoja)

## Goal

Endurecer el modo multi-hoja en `AppEditorPdf` para que la paginacion visual sea estable y performante bajo escenarios reales:

- typing continuo
- paste de bloques grandes
- imagenes que cargan tarde
- resize de viewport / cambio de zoom

## Constraints

- Documento continuo (una sola instancia de editor).
- Paginacion visual solamente (no modificar HTML persistido).
- Sin acoplarse a modulos consumidores.

## Strategies

### Scheduling

- Clasificar eventos:
  - typing simple: repaginacion deferred (debounced)
  - paste/cambios grandes: repaginacion immediate
  - imagen load/error: repaginacion immediate
- Evitar ejecutar repaginacion si ya hay una corrida activa (coalescing).

### Scroll & Selection Preservation

- Capturar anchor antes de repaginar.
- Restaurar scroll/seleccion al finalizar.
- Evitar re-posicionar el cursor durante locks (ej. interaccion con imagen).

### Observation Scope

- `ResizeObserver` sobre bloques top-level solo cuando se use paginacion visual.
- Limitar costo: si hay demasiados bloques, usar marca "dirtyStart" y revalidar desde ese punto.

## Non-Goals

- Cambiar API publica del componente para consumidores.
- Introducir export o persistencia final.

