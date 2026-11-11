# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Superficies UI

Propuesta UI-01: Importar carpetas…/Consultar importación… en menú superior, con operación disponible sin administrar perfiles. UI-02: contextual usa nivel pulsado elegible; superior usa nivel seleccionado. Ruta canónica y destino provienen del servidor. Expediente/documento/fondo vacío no importan por fallback.

UI-03 conserva asistente, referencia manual, último lote, verificación de confirmación sin acuse y avisos visibles. La franja permanente solo se retira tras verificar equivalencia. UI-04 incluye viewport, scroll y teclado/foco del plugin compartido. No se ha modificado aún el producto.

## Validacion visual

No ejecutada sobre una interfaz reorganizada: la implementación no ha comenzado. UI-05 deberá cubrir seleccionado A/contextual B, homónimos, permisos, ausencia de selección, reapertura/postback/recarga, recuperación, teclado y consumidores compartidos. Las 16 pruebas actuales ejercitan interfaz existente con servicios sintéticos; no certifican la propuesta visual.
