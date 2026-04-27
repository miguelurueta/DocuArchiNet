# Design: AppEditor Fase 2C - paste, listas, imagenes y hardening (SCRUMCORE-185)

## Objetivo

Endurecer el modo multipagina para casos de produccion:
- paste largo (con formato inline y links)
- listas basicas repartidas entre paginas
- imagenes como bloques indivisibles (mover completo a la siguiente pagina si no cabe)
- rendimiento razonable (incremental, sin recomputar todo por tecla)

## Estado actual

El editor ya cuenta con:
- motor de autoPagination (acciones before/list-item/split)
- reflow incremental desde `dirtyStartChildIndex`
- preservacion basica de seleccion/undo/redo
- soporte de listas e imagenes en el motor (tests existentes)

Fase 2C agrega pruebas de regresion y hardening en flujos de paste y documentos largos.

## Enfoque

- Expandir pruebas de paste:
  - paste largo con links (marks inline)
  - paste con listas (ul/ol/taskList si aplica)
  - paste con imagen grande (debe moverse completa)
- Asegurar que el reflow post-paste:
  - no corrompe el documento (sin perdida/duplicacion)
  - mantiene links y estructura de listas
  - mantiene imagen como bloque indivisible

Performance:
- mantener `autoPaginationDebounceMs` y la invalidez incremental ya existente.
- evitar loops infinitos: maxIterations ya presente.

