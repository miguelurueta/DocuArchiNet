# Arquitectura

La página `workflow/Webworkflow.aspx` mantiene dos consumidores mutuamente excluyentes. Con el gate apagado conserva el botón, modal, GridView y postbacks legacy. Con el gate activo para una audiencia autorizada oculta el disparador legacy, muestra la lista cronológica moderna e inicializa un único adaptador JavaScript.

El adaptador `WorkflowNotesModern` obtiene `idTarea` del campo explícito de la página, serializa con `JSON.stringify` y consume el ASMX moderno. Las reglas de autorización, propiedad, idempotencia, concurrencia y auditoría permanecen en servidor.

Los estilos están encapsulados bajo `.workflow-centro-trabajo-moderno`; no agregan dependencias ni modifican consumidores externos a `workflow/`.

Véase [diagrama de arquitectura](Diagramas/01-arquitectura.md).
