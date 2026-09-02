# ESTABILIZACION-WORKFLOW

- Ticket: DOC-44
- Cambio OpenSpec: doc-44-estabilizacion-workflow
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Se estabiliza exclusivamente el consumidor de Notas de `Webworkflow.aspx`: cliente único `WorkflowNotesModern`, tarea explícita, contrato ASMX DOC-42 reutilizado y exclusión mutua con legacy. No cambia esquema ni semántica de negocio.

## Alcance y compatibilidad

Paquete detallado: [DOC-44-ESTABILIZACION-WORKFLOW](../../../Actualizacion/workflow/Notas/DOC-44-ESTABILIZACION-WORKFLOW/00-indice.md).

Rollback: gate `false`, usuarios/grupos vacíos; no se revierten datos ni contratos DOC-42.
