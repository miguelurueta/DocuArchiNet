# Design: AppEditor Fase 2A - reflow incremental de parrafos (SCRUMCORE-182)

## Contexto

Base existente: "paginas reales" (Fase 1).  
Ticket: `SCRUMCORE-182` (Prompt 27-FE).

Este cambio apunta a un motor de reflow multipagina incremental sobre TipTap/ProseMirror:
- recalcular desde el bloque afectado hacia adelante
- partir parrafos por posicion real de texto
- empujar/traer contenido en cascada
- evitar paginas vacias intermedias
- mantener integridad del parrafo (sin perder ni duplicar texto)

## Estado actual (repo)

El AppEditor ya tiene:
- Modo `visual` con paginas reales y `pageBreak` (manual/auto).
- Motor `autoPagination` para insertar/remover `pageBreak` automaticos segun overflow.
- Invalidez incremental via `startChildIndex` y tracking de `dirtyStartChildIndex` en `useAppEditor`.

## Diseno propuesto (Fase 2A)

### Unidad de reflow

Top-level blocks del documento ProseMirror (p.ej. `paragraph`, `bulletList`, `image`, etc.).  
La paginacion incremental se inicia desde el primer bloque afectado (incluyendo el bloque anterior y el `pageBreak` auto previo cuando corresponde) para permitir:
- recomponer un parrafo partido
- traer contenido hacia arriba tras borrado
- evitar paginas vacias / breaks colgantes

### Split real de parrafos

Para textblocks, el motor debe escoger posiciones de split usando coordenadas reales:
- `coordsAtPos` + busqueda binaria para encontrar el ultimo `pos` cuyo bottom <= boundary
- ajustar a corte preferido (espacio) cuando exista
- permitir split por caracter cuando no hay espacio cercano

### Normalizacion

Tras aplicar acciones de paginacion:
- sincronizar `spacerHeight` de pageBreaks auto
- limpiar pageBreaks auto redundantes
- evitar dos pageBreaks consecutivos
- eliminar paginas vacias intermedias (equivalente: remover breaks auto que no separan contenido real)

### Performance / restriccion "no recomputar todo"

- Mantener invalidacion incremental:
  - por transaccion (seleccion / docChanged) ya existe
  - por cambios de altura reales (ResizeObserver) para imagenes / reflow de layout
- Debounce por frame / por ventana de ms (ya existe `autoPaginationDebounceMs`)

## Casos obligatorios (enfoque)

1. Escribir al final de pagina:
   - insertar split/break para continuidad en pagina siguiente sin perder cursor.
2. Editar un parrafo ya partido:
   - remover breaks desde posicion de limpieza y recalcular splits correctos.
3. Crecer el parrafo y empujar resto:
   - cascada de acciones desde el bloque afectado hacia adelante.
4. Borrar y subir contenido:
   - remover breaks auto innecesarios y recomponer contenido hacia arriba.

## Riesgos

- Off-by-one en posiciones ProseMirror al medir/splitear puede duplicar o perder caracteres.
- Cambios de zoom/estilos alteran mediciones y gatillan cascadas; requiere invalidacion correcta.
- Listas complejas e imagenes quedan fuera del hardening, pero no deben romper integridad.

