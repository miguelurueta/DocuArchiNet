# Decisión de flujo o ruta

```mermaid
flowchart TD
    Inicio([idTarea]) --> Contexto{Contexto y gate activos?}
    Contexto -- No --> B1[DTO bloqueado<br/>sin destinos]
    Contexto -- Sí --> Tarea{Tarea activa y<br/>autorizada?}
    Tarea -- No --> B2[WORKFLOW_TASK_UNAVAILABLE]
    Tarea -- Sí --> Tipo{TipoDecision}
    Tipo -- FLUJO --> Conectores[Filtrar conectores por<br/>origen y usuario real]
    Tipo -- RUTA --> Docuarchi[Leer estado de trámite<br/>en Docuarchi]
    Docuarchi --> Ruta{Ruta y trámite<br/>abiertos?}
    Ruta -- No --> B4[WORKFLOW_ROUTE_CLOSED]
    Ruta -- Sí --> Actividades[Filtrar por ruta,<br/>grupo y actividad origen]
    Tipo -- Otro --> B5[WORKFLOW_TRANSITION_INCONSISTENT]
    Conectores --> Destinos{¿Hay destinos?}
    Actividades --> Destinos
    Destinos -- No --> B6[WORKFLOW_NO_DESTINATIONS]
    Destinos -- Sí --> Ok[PrevisualizacionTransicionDto]
```

Todos los filtros reciben el contexto resuelto en servidor; el flujo nunca toma permisos ni origen desde el navegador. Para `RUTA`, solo el estado del trámite se consulta en Docuarchi; los destinos se mantienen en Workflow.
