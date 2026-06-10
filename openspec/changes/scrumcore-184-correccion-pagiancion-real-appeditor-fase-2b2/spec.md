# Spec: SCRUMCORE-184 Fase 2B2 - cursor/seleccion/links

## Reglas

1. Cursor coherente:
   - Tras reflow, el cursor queda en posicion logica cercana (mismo punto semantico).
2. Seleccion estable:
   - Selecciones cerca del corte de pagina sobreviven split/merge.
3. Marks inline:
   - `link`, `bold`, `italic`, `underline` se conservan tras split/merge.
4. Undo/redo:
   - Undo/redo no corrompe el documento en casos cubiertos.

## Casos obligatorios

- Cursor al escribir en parrafo partido
- Seleccion cerca del corte de pagina
- Link que cruza el punto de split
- Marks inline en reflow
- Undo/redo basico despues del reflow

## Criterios de aceptacion

- Cursor coherente (sin saltos obvios)
- Links y formato inline sobreviven split/merge

