# Prompt: 29-FE AppEditor Fase 2B seleccion cursor y links

Actua como arquitecto y desarrollador senior especializado en TipTap, ProseMirror, seleccion de texto y marks inline.

Necesito implementar `AppEditor Fase 2B - seleccion, cursor y links` sobre el motor de reflow ya creado.

## Objetivo
- preservar cursor por posicion logica del documento
- estabilizar seleccion durante split y merge
- conservar links y formato inline

## Restricciones
- no usar timeouts como mecanismo base de cursor
- no romper transacciones de ProseMirror
- no perder marks inline al partir contenido

## Casos obligatorios
- cursor al escribir en parrafo partido
- seleccion cerca del corte de pagina
- link que cruza el punto de split
- negrita, cursiva y subrayado en reflow
- undo/redo basico despues del reflow

## Entregables
- implementacion real de mapping de seleccion
- preservacion de marks inline
- pruebas de cursor y links

## Criterios de aceptacion
- el cursor permanece coherente despues del reflow
- links y formato inline sobreviven al split/merge
- la experiencia de edicion ya no presenta saltos obvios

