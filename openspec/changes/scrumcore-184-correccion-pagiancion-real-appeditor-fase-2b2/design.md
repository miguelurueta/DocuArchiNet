# Design: AppEditor Fase 2B2 - seleccion, cursor y links (SCRUMCORE-184)

## Contexto

Este ticket vuelve a pedir Fase 2B (seleccion/cursor/links), con foco en "implementacion real" + pruebas.
Base: motor de reflow multipagina ya implementado (Fase 2A).

## Objetivo

- Preservar cursor por posicion logica del documento
- Estabilizar seleccion durante split/merge
- Conservar links y formato inline (marks)
- Undo/redo basico estable tras reflow

## Restricciones

- No usar timeouts como mecanismo base
- No romper transacciones ProseMirror
- No perder marks inline al partir contenido

## Propuesta tecnica

### 1) Captura y restauracion de seleccion (logica)

Antes del reflow:
- Capturar `Selection` actual:
  - `from/to`
  - si es `NodeSelection`, capturar identidad del nodo (attrs relevantes)
- Capturar contexto:
  - pagina actual (solo para scroll UX)

Durante reflow:
- Preferir operaciones que preserven seleccion cuando se ejecutan splits/insertions.
- Evitar agregar al history cambios mecanicos (spacerHeight).

Despues del reflow:
- Resolver una seleccion valida:
  - clamping al `doc.content.size`
  - si existio mapping de transaction, preferir `mapping.map(from/to)`
  - si el nodo era una imagen, re-resolver por identidad

### 2) Preservacion de marks inline

En split/merge de textblocks:
- Asegurar que los pasos de ProseMirror mantengan marks en el contenido que se mueve.
- Para links: preservar attrs (href, target) y evitar recortes que dejen texto "medio link".

### 3) Pruebas

Agregar tests de integracion sobre `useAppEditor` y/o comandos del editor:
- cursor al escribir en parrafo partido
- seleccion cerca del corte
- link que cruza split
- bold/italic/underline en reflow
- undo/redo basico post-reflow

