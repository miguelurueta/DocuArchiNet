# FIX-SELLOS-SII

- Ticket: DOC-84
- Cambio OpenSpec: doc-84-fix-sellos-sii
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

La superficie afectada es el modal oficial de importación en `workflow/Webworkflow.aspx` y `GridView_list_documento_relacion_wf`. La fila se agrega con los ocho campos del contrato legacy y se deduplica por `id_wf`. El modal permanece abierto si la fila no puede comprobarse; foco, selección, responsive y acciones ENLASE no cambian.

El selector de tipología propone automáticamente una única variante autorizada de Constancia de Inscripción para el lote. El usuario puede cambiarla; si el catálogo contiene dos variantes equivalentes, ninguna se selecciona automáticamente.

## Validacion visual

La operabilidad contractual fue validada con pruebas DOM determinísticas. La aceptación visual autenticada quedó deliberadamente no ejecutada por falta de autorización específica de ambiente y cuentas.
