# Spec: Correccion paginacion real AppEditor (Fase 2A)

## Objetivo

Implementar paginacion real y reflow incremental de parrafos en AppEditor.

## Definiciones

- **Item**: unidad de contenido paginable (parrafo/bloque) con `id` estable.
- **Pagina**: contenedor con alto fijo (`pageHeightPx`) y padding.
- **Layout**: asignacion de cada item a (`pageIndex`, `offsetTopInPagePx`) en orden.
- **Reflow**: recalculo de layout a partir de un indice `i0`.

## Reglas funcionales

1. La paginacion se calcula con base en alturas medidas del DOM por item.
2. La asignacion de items a paginas debe ser determinista con el mismo input:
   - mismo orden de items,
   - mismas medidas de items,
   - mismos parametros de pagina.
3. Al cambiar un item (contenido o altura), el sistema recalcula desde el primer item afectado.
4. Si cambian parametros globales (alto de pagina, margenes), el sistema recalcula desde el inicio.
5. Items que exceden `usablePageHeight`:
   - Se colocan al inicio de una pagina nueva si no estan ya ahi.
   - Se marcan como `oversize: true` para debug y para futuros refinamientos.

## Requisitos no funcionales

- Rendimiento: para N items, el reflow incremental debe ser O(K) donde K << N para ediciones locales.
- Estabilidad: no debe cambiar layout sin una causa (delta en item height o page params).
- Observabilidad: debe existir un modo debug para inspeccionar boundaries y asignaciones.

## Entradas / Salidas (modelo propuesto)

### Input

- `items: Array<{ id: string }>` (ordenado)
- `measuresById: Record<string, number>` (alto px, entero)
- `page: { heightPx: number, paddingTopPx: number, paddingBottomPx: number }`

### Output

- `layout: Array<{ id: string, heightPx: number, pageIndex: number, offsetTopPx: number, oversize?: boolean }>`
- `pagesCount: number`

## Eventos que disparan reflow

- `ITEM_HEIGHT_CHANGED(id, newHeightPx)`
- `ITEMS_ORDER_CHANGED(...)` (por ahora: reflow total)
- `PAGE_PARAMS_CHANGED(...)` (reflow total)

## Criterios de prueba (aceptacion)

- Caso 1: editar un parrafo en la mitad del documento:
  - se recalculan paginas desde ese item y los posteriores, no los anteriores.
- Caso 2: cambiar `pageHeightPx`:
  - se recalcula todo desde el inicio.
- Caso 3: item oversize:
  - queda marcado `oversize` y su `offsetTopPx` es 0.

