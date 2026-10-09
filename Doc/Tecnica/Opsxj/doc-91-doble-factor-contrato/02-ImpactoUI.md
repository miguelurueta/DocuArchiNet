# DOBLE-FACTOR-CONTRATO

- Ticket: DOC-91
- Cambio OpenSpec: doc-91-doble-factor-contrato
- Clasificacion: cross_cutting

## Superficies UI

DOC-91 no crea ni modifica páginas, controles, scripts, estilos, modales o mensajes visibles. Los DTO se preparan para una frontera futura, pero no existe endpoint ni consumidor de interfaz en esta entrega.

## Validacion visual

No aplica validación visual porque la fundación permanece desconectada de producción. La comprobación de no regresión consiste en demostrar mediante diff y búsqueda estructural que `gestor.aspx` y sus recursos no cambian ni referencian los símbolos nuevos.
