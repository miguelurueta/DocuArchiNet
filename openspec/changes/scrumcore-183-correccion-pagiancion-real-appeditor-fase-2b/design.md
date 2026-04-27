# Design: AppEditor Fase 2B - seleccion, cursor y links (SCRUMCORE-183)

## Contexto

Dependencia: Fase 2A (reflow incremental de parrafos / paginacion real).  
Este ticket (2B) endurece UX: seleccion/cursor y marks inline (links, bold, italic, underline) durante split/merge y reflow.

## Problema

Durante reflow multipagina, el documento se modifica (insert/remove `pageBreak`, split de textblocks, merges al remover breaks). Esto puede:
- mover el cursor a posiciones inesperadas
- romper seleccion al cruzar el corte de pagina
- perder/duplicar marks inline (especialmente links) al partir o recomponer parrafos
- afectar undo/redo si los cambios automaticos se agregan al historial de forma inadecuada

## Enfoque tecnico

### 1) Preservacion de seleccion/cursor (posicion logica)

Objetivo: mantener una "seleccion logica" a traves de las transacciones de reflow.

Estrategia:
- Capturar `Selection` antes de la limpieza/reflow:
  - rango `{ from, to }`
  - si hay `NodeSelection` (imagen), identidad (attrs) para re-resolver.
- Ejecutar limpieza/reflow con `preserveSelection: true` cuando sea posible.
- Tras reflow:
  - clamp `from/to` al nuevo `doc.content.size`
  - si se partio/mergeo el textblock que contenia la seleccion, usar el mapeo de transacciones para aproximar.

Nota: ya existe infraestructura en `useAppEditor.ts` para preservar seleccion en reflow; este cambio busca robustecerla para marks y cortes.

### 2) Marks inline robustos (links/bold/italic/underline)

Los splits/merges deben:
- conservar marks al mover texto entre nodos
- evitar "cortar" el mark dejando HTML inconsistente
- rehidratar links dentro de paginas reales en modo visual

En ProseMirror, la preservacion de marks depende de:
- como se construyen los nodos al split (mantener `marks` en el contenido resultante)
- como se mergea al remover pageBreak auto (`mergeOnRemove`)

### 3) Undo/Redo estable

Regla: cambios automaticos de paginacion no deben romper undo/redo.

Estrategia:
- Mantener `addToHistory=false` para ajustes puramente mecanicos (p.ej. sync de spacerHeight).
- Evitar que un solo "teclazo" implique N entradas de historia por autoPageBreaks intermedios.
- Asegurar que tras undo/redo, el motor pueda re-ejecutar reflow sin corrupcion.

## Casos de validacion minima

- Editar link en parrafo partido.
- Escribir con negrita/cursiva en punto de corte.
- Seleccionar texto entre final de pagina y comienzo de la siguiente.
- Undo/redo despues de reflow.

