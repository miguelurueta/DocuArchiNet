# Arquitectura

El consumidor vive en `Webworkflow.aspx`; `WorkflowNotesModern` es su único adaptador. Las seis operaciones llaman `WebServiceWorkflowNotesModern.asmx` con `idTarea` explícito. El servicio compartido, autorización backend y tablas DOC-42 se reutilizan sin cambios.

La presentación se decide en `ConfigureWorkflowNotesModernPresentation`: gate activo muestra `Panel_notas_modernas` y oculta `Panel_Buttonanotacion`; gate apagado hace lo contrario y no registra el bootstrap moderno.

Véase [diagrama de arquitectura](Diagramas/01-arquitectura.md).
