# Impacto UI — Verificación transversal

- Ticket: DOC-38
- Cambio OpenSpec: doc-38-verificacion-transversal
- Clasificacion: cross_cutting

## Superficies UI

La superficie revisada es el Centro de trabajo en `workflow/Webworkflow.aspx`. La operación Devolver → Usuario anterior usa sus módulos de interfaz propios para abrir el preview, mostrar el contexto mínimo, cancelar o confirmar la ejecución y restaurar la bandeja al finalizar.

La verificación confirma que esta operación no evalúa `WorkflowCentroTrabajoModernActive`, no usa postback, campos ocultos ni handlers Web Forms para autorizar o ejecutar. También comprueba que no abre, invoca ni sustituye la devolución a actividad anterior al cambiar de tarea o al reemplazarse el contenido parcial de la bandeja.

## Validacion visual

La QA manual no autenticada revisará el recorrido de preview, confirmación, cancelación, bloqueo funcional, estado de espera, foco, teclado, responsive y restauración del desplazamiento horizontal. El resultado se documentará con pasos reproducibles y referencias saneadas; no se usarán credenciales ni se ejecutará una transición real como parte de esta etapa.

Una interfaz visualmente correcta no aprueba por sí sola DOC-38: debe coincidir con evidencia de contrato, lock, token, aislamiento y no regresión de las demás operaciones Workflow.
