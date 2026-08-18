# PILOTO-DESPLIGUE-CONTROLADO

- Ticket: DOC-14
- Cambio OpenSpec: doc-14-piloto-despligue-controlado
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

- `ConfiguracionWorkflowModernFeatureGate` evalúa bandera, exclusiones, alcance piloto, modo oficial, metadatos y rollback con comportamiento *fail closed*.
- `WebServiceWorkflowModern.asmx` y `ServicioTransicionTarea` revalidan el gate antes de preview o ejecución; una llamada no habilitada no invoca el ejecutor legacy.
- `ServicioTransicionTarea` conserva el guard de concurrencia y registra éxito, bloqueo o error mediante `IAuditoriaTransicionRepository`; un fallo de auditoría agrega una advertencia segura sin alterar el resultado funcional.
- `WorkflowLegacyAuditoriaAdapter` normaliza el contrato mínimo permitido y es la única frontera que persiste telemetría en la bitácora legacy.
