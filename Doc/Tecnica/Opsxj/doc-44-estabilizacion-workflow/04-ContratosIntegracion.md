# ESTABILIZACION-WORKFLOW

- Ticket: DOC-44
- Cambio OpenSpec: doc-44-estabilizacion-workflow
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Las seis operaciones modernas envían `idTarea`; consulta/edición/eliminación envían `idNota` y las mutaciones de actualización/eliminación incluyen `version`. Crear incorpora `clientRequestId`. El transporte usa JSON real y sesión autenticada de mismo origen.

Contrato completo: [02-contrato.md](../../../Actualizacion/workflow/Notas/DOC-44-ESTABILIZACION-WORKFLOW/02-contrato.md). Impacto de esquema: ninguno.
