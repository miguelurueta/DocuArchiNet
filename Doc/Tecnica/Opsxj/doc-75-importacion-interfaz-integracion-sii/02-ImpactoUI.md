# IMPORTACION-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-75
- Cambio OpenSpec: doc-75-importacion-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- [x] `workflow/Webworkflow.aspx`: popup secundario dentro del modal y acciones por fila/selección en la tabla SII.
- [x] Estados `edicion/preparando/listo/creando/creado/bloqueado`; foco restaurado, anuncios `aria-live`, selección explícita y layout móvil de una columna.

## Validacion visual

Revisión local reproducible: abrir modal con datos controlados, preparar una fila y selección múltiple, verificar catálogo, plan rotulado como previsto, cancelación/foco y confirmación deshabilitada sin datos. No se ejecutó E2E autenticada por falta de autorización específica para DOC-75.
