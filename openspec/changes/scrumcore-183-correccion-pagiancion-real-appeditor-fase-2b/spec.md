# Spec: SCRUMCORE-183 AppEditor Fase 2B - seleccion, cursor y links

## Objetivo

Hacer estable la seleccion/cursor durante reflow multipagina y preservar marks inline, especialmente links.

## Reglas funcionales

1. Cursor estable:
   - Despues de reflow, el cursor debe permanecer en una posicion logica cercana a la original.
   - No debe saltar a inicio/fin arbitrariamente salvo clamp por limites del doc.
2. Seleccion estable:
   - Seleccionar a traves del corte de pagina debe funcionar sin perder rango.
3. Marks inline:
   - Al partir o recomponer parrafos, se conservan `link`, `bold`, `italic`, `underline` sobre el texto correspondiente.
   - Links no deben romperse (sin perder `href`/attrs) al split/merge.
4. Undo/redo:
   - Undo/redo no debe corromper el documento en escenarios de reflow cubiertos.

## Requisitos no funcionales

- El reflow no debe degradar la escritura normal.
- Cambios automaticos deben minimizar ruido en history.

