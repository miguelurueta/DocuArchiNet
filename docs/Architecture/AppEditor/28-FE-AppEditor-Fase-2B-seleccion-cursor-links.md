# Ticket: 28-FE AppEditor Fase 2B seleccion cursor y links

## Identificacion
- Cambio: `28-FE`
- Nombre: `AppEditor Fase 2B - seleccion, cursor y links`
- Ticket propuesto: `SCRUMCORE-AE-F2B-01`
- Modulo: `src/app/Components/UI/AppEditor/`
- Dependencia previa: `26-FE AppEditor Fase 2A - reflow incremental de parrafos`

## Objetivo
Hacer estable la seleccion y el cursor durante el reflow multipagina, preservando formato inline y links.

## Alcance
- mapping de cursor por posicion logica
- seleccion estable al cruzar paginas
- preservacion de marks inline al partir parrafos
- links, bold, italic y underline robustos tras split/merge
- estabilidad basica de undo/redo en escenarios de reflow

## No alcance
- IME avanzado de todos los idiomas
- tablas complejas
- comentarios o anotaciones

## Criterios de aceptacion
- el cursor no salta a posiciones ilogicas
- seleccionar cerca del corte de pagina funciona
- links no se rompen al partir un parrafo
- el formato inline se conserva al crecer o reducir contenido
- undo/redo no corrompe el documento en casos cubiertos

## Validacion minima
- editar link en parrafo partido
- escribir con negrita/cursiva en punto de corte
- seleccionar texto entre final de pagina y comienzo de la siguiente
- undo/redo despues de reflow

