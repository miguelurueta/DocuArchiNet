# Flujo y seguridad

- Ticket: DOC-9
- Cambio OpenSpec: doc-9-contrato-terminar-tarea-workflow
- Clasificacion: cross_cutting

## Secuencia de DOC-9

```text
Composición futura -> ContextoModuloWorkflow validado -> IWorkflowModernFeatureGate
    -> inactivo/excluido: retorno funcional, sin lectura ni transición
    -> activo: ServicioTransicionTarea y puertos tipados disponibles
    -> ejecución solicitada en DOC-9: WORKFLOW_MODERN_EXECUTION_PENDING
```

La fundación no tiene superficie de red. Ningún navegador puede invocar estas clases y ningún código nuevo llama al motor legacy.

La única secuencia ejecutable en DOC-9 termina antes del motor legacy: `ServicioTransicionTarea` valida contexto y solicitud, consulta los puertos tipados y entrega `WORKFLOW_MODERN_EXECUTION_PENDING` cuando se intenta ejecutar. No hay ASMX, JavaScript, composición Web Forms ni actualización de estado de tarea en esta fase.

## Límites obligatorios

- Presentation futura construirá el contexto autorizado; Application y Domain no conocen `Page`, `Session`, `GridView`, `UpdatePanel` ni `ModalPopupExtender`.
- Solo `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb` puede ser ampliado para llamar a `Terminar_Tarea_Workflow` y `Cambia_Estado`.
- `PRETERMINARACTIVIAD` y `TERMINARACTIVIDAD` siguen dentro del motor legacy y no se reproducen en DTOs o JavaScript.
- La infraestructura compartida recibe `ContextoModulo` validado y trabaja con comandos parametrizados; Workflow entrega su contexto especializado sin filtrar `DataSet` ni HTML.
- Las implementaciones futuras de repositorios se limitan a `Infrastructure/Repositories/Workflow/`; la infraestructura común de datos no conoce `ContextoModuloWorkflow` ni códigos `WORKFLOW_*`.
- Los códigos funcionales son estables; no contienen conexiones, SQL, rutas, credenciales ni trazas.

## Riesgos y mitigaciones

| Riesgo | Mitigación de DOC-9 |
|---|---|
| Activación accidental del camino nuevo | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` evalúa `WorkflowCentroTrabajoModernActive` con fallo cerrado; la bandera visual existente no se reutiliza. |
| Duplicar validaciones criticales | No hay SQL ni llamada nueva al motor; el adaptador aún bloquea. |
| Doble transición o concurrencia | Sin endpoint no hay mutación. Las futuras fases deberán revalidar token, pertenencia y conector. |
| Filtrado de información interna | DTOs específicos y errores funcionales; sin excepciones serializadas. |

## Compatibilidad, piloto y rollback

El piloto actual de la interfaz no se modifica. DOC-9 se revierte retirando los nuevos archivos o dejando la composición sin uso: no hay cambio de configuración requerido, migración de datos ni alteración del flujo legacy. Una fase posterior debe habilitar explícitamente el gate, comparar resultados con el camino actual y definir rollback antes de enrutar usuarios modernos.
