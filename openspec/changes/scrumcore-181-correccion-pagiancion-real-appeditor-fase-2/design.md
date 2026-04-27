# Design: Correccion paginacion real AppEditor (Fase 2)

## Contexto

Ticket: `SCRUMCORE-181`  
Tema: "AppEditor Fase 2A reflow incremental de parrafos" y "paginacion real".

El `proposal.md` actual es un placeholder generado desde Jira y no trae detalle funcional; este design define supuestos y una ruta de implementacion incremental para poder avanzar sin el CLI de OpenSpec.

## Problema

En el AppEditor, los parrafos/fragmentos se renderizan en un contenedor con "paginacion". Hoy se observan errores de paginacion real (breaks incorrectos, saltos de pagina inconsistentes, reflow completo costoso, resultados distintos segun zoom/medidas, etc.).

Objetivo: lograr una paginacion determinista y estable, y actualizarla de forma incremental (solo lo afectado) cuando cambian parrafos o el layout.

## Alcance (Fase 2A)

- Reflow incremental de parrafos: recalcular layout/paginacion a partir del primer parrafo afectado.
- Paginacion real basada en medidas reales de DOM (no estimadas) con reglas claras.
- Instrumentacion de diagnostico para reproducir/validar (modo debug).

Fuera de alcance (por ahora):
- Export/PDF.
- Paginacion "perfecta" a nivel de palabra/linea si el editor es altamente dinamico (se aborda por iteraciones).
- Optimizaciones agresivas (virtualizacion avanzada) si no es necesario.

## Supuestos (a validar)

- Existe un "AppEditor" con contenido por parrafos (o bloques) renderizados en React.
- Hay un concepto de pagina (alto fijo) y margenes, y la app muestra N paginas.
- Se puede medir alto de cada parrafo/bloque en DOM (por ejemplo via `ResizeObserver`, `getBoundingClientRect`, refs).
- El estado del editor permite saber cual parrafo cambió (por id/posicion).

## Propuesta tecnica

### Representacion de layout

Mantener un modelo de layout derivado de:
- `pageHeightPx`, `pagePaddingTopPx`, `pagePaddingBottomPx`, `pageGapPx` (si aplica)
- Lista ordenada de items (parrafos/bloques) con:
  - `id`
  - `measuredHeightPx`
  - `pageIndex`
  - `offsetTopInPagePx`

### Algoritmo de paginacion (greedy)

Para cada item en orden:
1. Si `offset + itemHeight <= usablePageHeight`: se ubica en la pagina actual.
2. Si no cabe: se mueve a la siguiente pagina, `offset = 0`.
3. Se incrementa `offset += itemHeight`.

Notas:
- Se necesita definir `usablePageHeight = pageHeightPx - paddingTop - paddingBottom`.
- Para items mayores que una pagina (overflow):
  - Estrategia Fase 2A: permitir overflow visual o forzar item a nueva pagina y marcarlo como "oversize".
  - Luego: split por lineas/fragmentos si el editor lo soporta.

### Reflow incremental

Cuando cambia el alto de un item, o cambian parametros de pagina:
- Encontrar el primer indice afectado `i0`:
  - Si cambio de un item: su indice.
  - Si cambio de layout (pageHeight): `i0 = 0`.
- Recalcular paginacion desde `i0` hasta que:
  - `pageIndex` y `offsetTop` de items deja de cambiar respecto al layout previo por un tramo suficiente (heuristica),
  - o se alcanza el final.

Guardrail: si hay incertidumbre o el delta toca muchos items, se permite fallback a recalculo total.

### Medicion

Opciones de medicion:
- `ResizeObserver` por item (mejor incremental).
- Medicion on-demand con `requestAnimationFrame` tras cambios.

Recomendacion Fase 2A:
- `ResizeObserver` para detectar cambios de altura.
- En la capa de layout, debouncing por frame (agrupar multiples cambios en 1 recalculo).

### Debug / Telemetria (solo dev)

Agregar un flag (por ejemplo query param o env) que muestre:
- page boundaries
- indices y offsets
- item heights
- markers de "oversize"
- logs: `reflowStartIndex`, `itemsRecomputed`, tiempo total (ms)

## Riesgos

- Cambios de altura por fuentes/zoom, o carga de imagenes, provocan cascadas de reflow.
- Medicion de DOM puede variar por rounding; hay que normalizar (p.ej. `Math.round`).
- Performance si hay muchos observers; mitigacion: solo observar items visibles o agrupar.

## Criterios de aceptacion (Fase 2A)

- Al editar un parrafo, no se recalcula desde 0 siempre; se recalcula desde el primer parrafo afectado.
- Paginacion estable: mismo contenido + mismo page size produce mismas paginas.
- No hay "saltos" aleatorios al re-render (sin cambios).
- Instrumentacion habilitable para verificar layout y offsets.

