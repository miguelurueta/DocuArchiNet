# PILOTO-DESPLIGUE-CONTROLADO

- Ticket: DOC-14
- Cambio OpenSpec: doc-14-piloto-despligue-controlado
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

- Los endpoints `PreviewEnviarTarea` y `EjecutarEnvioTarea` conservan la autenticación y revalidan `IWorkflowModernFeatureGate` en servidor antes de consultar o terminar una tarea.
- La respuesta expone solo estado y código funcional seguro (`WORKFLOW_MODERN_ACTIVE`, `WORKFLOW_MODERN_INACTIVE`, `WORKFLOW_MODERN_EXCLUDED` y fallbacks definidos); no incluye configuración, credenciales, tokens, Session ni payload sensible.
- `AuditoriaTransicion` transporta correlación, identidad autorizada, tarea, ruta/flujo, conector, canal, duración, resultado y código funcional. No requiere cambios de esquema ni una integración de persistencia paralela.
- El adaptador legacy mantiene compatibilidad con la bitácora existente; el rollback solo cambia configuración y no ejecuta SQL ni reversión de negocio.
