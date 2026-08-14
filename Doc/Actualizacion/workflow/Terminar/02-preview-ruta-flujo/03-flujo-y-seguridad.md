# Flujo y seguridad

Los diagramas renderizables de uso, clases, secuencia, estados y decisión se encuentran en [Diagramas](Diagramas/). La secuencia completa es [01-secuencia-preview.md](Diagramas/01-secuencia-preview.md) y los estados observables son [04-estados-preview.md](Diagramas/04-estados-preview.md).

Antes del feature gate, `WorkflowPreviewSessionContextGate` valida la sesión existente y, para cada llamada de Gestión autenticada, valida la relación Gestión → Workflow con lecturas controladas, establece únicamente las claves Workflow relacionadas y entrega snapshots de conexión Workflow/Docuarchi al ASMX. No registra auditoría ni llama el inicializador legacy completo. Después, el feature gate se evalúa antes de consultar tarea, flujo o ruta. La tarea se filtra por usuario Workflow, selección activa y estado vigente; ruta y flujo se filtran con el grupo/origen reales resueltos en servidor. La ruta consulta `tipo_doc_entrante` en Docuarchi y sus destinos en Workflow.

La lectura de conectores de flujo no utiliza `TIPO_RUTA_ABIERTA_CERRADA` ni `TIPO_ABIERTA_CERRADA_ACTIVIDAD` como veto. Ambos campos representan libertad de asignación; la selección de destinos conserva los conectores salientes autorizados del origen real.

El token se devuelve para una fase posterior de envío, pero DOC-10 no lo consume ni escribe estado. Una llamada repetida es de lectura y debe producir la misma observación mientras no cambie el estado legacy.

## Riesgos y rollback

- Un error inesperado del endpoint se convierte en `WORKFLOW_TRANSITION_INCONSISTENT`, sin detalle interno.
- Desactivar `WorkflowCentroTrabajoModernActive` bloquea inmediatamente nuevas llamadas modernas; no requiere migración ni reversión de datos.
- La concurrencia se conserva como responsabilidad del flujo legacy. La E2E compara tarea, estado y auditoría antes/después para comprobar que preview no muta.
- La decisión de modernizar a asincronía requiere medición. `tools/e2e/scripts/run-doc10-concurrency.cjs` crea sesiones Gestión reales independientes con login dosificado y mide únicamente el POST del ASMX con niveles 20 y 30; registra p50/p95/p99, fallos y huellas antes/después. Una solicitud por sesión evita medir la serialización de la misma sesión en lugar de la concurrencia del endpoint.
- El rollback funcional es configuración del gate; nunca se ejecuta SQL ni `Cambia_Estado` para revertir un preview.
