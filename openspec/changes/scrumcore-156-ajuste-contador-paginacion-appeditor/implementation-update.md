# Implementation Update: SCRUMCORE-156

## Resumen

La ejecucion final del ticket amplio el alcance implementado respecto del ajuste
inicial del contador.

Ademas de estabilizar `currentPage`, el cambio retira la repaginacion
destructiva de `useAppEditor` como mecanismo principal del modo visual y mueve
la responsabilidad del layout paginado a una capa derivada.

## Cambios principales

- `useAppEditor` deja de insertar y limpiar `autoPageBreak` para sostener la
  paginacion visual en el flujo normal de edicion
- `usePaginationMetrics` concentra el layout visual derivado y aplica
  desplazamientos visuales a bloques indivisibles cuando deben pasar a la hoja
  siguiente
- el gap entre hojas se resuelve visualmente sin reescribir el documento
  TipTap
- `usePageContext` conserva el calculo desacoplado de pagina actual sobre
  `scroll`, boundaries visuales y `zoom`

## Efecto arquitectonico

El ticket deja de ser solamente un ajuste de contador y pasa a cubrir tambien
el desacoplamiento del flujo visual principal respecto del motor destructivo
previo.

El documento editable permanece continuo y la representacion de paginas pasa a
depender del layout visual derivado, no de mutaciones correctivas del contenido
como comportamiento base.
