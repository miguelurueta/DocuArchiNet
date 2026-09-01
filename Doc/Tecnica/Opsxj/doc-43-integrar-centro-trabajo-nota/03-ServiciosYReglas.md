# INTEGRAR-CENTRO-TRABAJO-NOTA

- Ticket: DOC-43
- Cambio OpenSpec: doc-43-integrar-centro-trabajo-nota
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

Documentar clases VB.NET/C#, reglas de negocio, dependencias y manejo de errores.

## Inventario confirmado

- `WebServiceWorkflowNotesModern` expone lectura y mutaciones de Notas.
- `MySqlNotasWorkflowRepository` usa `FabricaConexion` y `EjecutorDatos` heredados del repositorio base.
- El esquema aplicado en `workflowdocument` y `workflowtconta` incluye InnoDB, índices operativos/históricos, `workflow_notas_idempotencia`, `Version_Resultado` y `workflow_notas_version`.
- Las mutaciones de actualización y eliminación son condicionales por `Version`; el resultado de versión obsoleta es `VersionConflict`.
