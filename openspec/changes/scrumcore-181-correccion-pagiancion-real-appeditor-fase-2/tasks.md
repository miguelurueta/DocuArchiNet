# Tasks: SCRUMCORE-181 Correccion paginacion real AppEditor (Fase 2A)

## Preparacion

- [ ] Identificar el modulo/componente del AppEditor donde se calcula la paginacion actual.
- [ ] Identificar la unidad paginable actual (parrafo/bloque) y confirmar si tiene `id` estable.
- [ ] Confirmar parametros de pagina (alto, padding, unidades) y donde se configuran.

## Modelo de layout

- [ ] Definir tipos/interfaces de layout (items, page params, layout result).
- [ ] Implementar funcion pura `paginate(items, measuresById, pageParams, startIndex?)`.
- [ ] Incluir manejo `oversize` para items mayores al alto util.

## Medicion incremental

- [ ] Implementar medicion de altura por item con `ResizeObserver` (o estrategia equivalente).
- [ ] Normalizar alturas a enteros (p.ej. `Math.round`) para evitar jitter por subpixeles.
- [ ] Debounce por frame: agrupar multiples cambios de altura en un solo reflow.

## Reflow incremental

- [ ] Mantener un cache del layout previo para comparar cambios.
- [ ] Al recibir height change de un item, encontrar su indice y recalcular desde alli.
- [ ] Definir heuristica de corte (cuando el layout deja de cambiar) o fallback a recalculo total.

## UI / Integracion

- [ ] Renderizar paginas segun `layout` y `pagesCount`.
- [ ] Asegurar que la UX no parpadea: preservar scroll/selection si aplica.

## Debug

- [ ] Agregar flag de debug (env o query) para mostrar boundaries y metadatos.
- [ ] Loggear metricas de reflow (startIndex, recomputedCount, durationMs) en modo debug.

## Validacion

- [ ] Agregar pruebas unitarias para `paginate` (casos basicos, oversize, startIndex).
- [ ] Agregar un escenario manual reproducible (documento de ejemplo) para verificar paginacion estable.

