# PILOTO-DESPLIGUE-CONTROLADO

- Ticket: DOC-14
- Cambio OpenSpec: doc-14-piloto-despligue-controlado
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Habilitar de forma controlada la presentación moderna del centro de trabajo sin sustituir el motor legacy. El gate único distingue un piloto por inclusión explícita de un modo oficial explícito; ambos requieren metadatos operativos y fallan a legacy ante una configuración inválida.

## Alcance y compatibilidad

- Superficies afectadas: `workflow/Webworkflow.aspx`, `WorkflowModernPresentationBootstrap`, `WebServiceWorkflowModern.asmx`, `ServicioTransicionTarea`, el gate de Infrastructure y la auditoría legacy.
- Se preservan `Terminar_Tarea_Workflow`, `Cambia_Estado`, autorización, firma, expediente, eventos, correo, transacciones y trazabilidad legacy.
- El rollback por `tools/validation/Invoke-Doc14Rollback.ps1` desactiva las banderas moderna y oficial, limpia el alcance piloto y no revierte transiciones confirmadas.
- La configuración actual de la raíz canónica usa modo oficial explícito, con listas piloto vacías, responsable, motivo y fecha registrados en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`.
