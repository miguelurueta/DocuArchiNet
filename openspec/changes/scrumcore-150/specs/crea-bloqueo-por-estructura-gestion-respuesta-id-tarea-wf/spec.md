# Delta Spec: crea-bloqueo-por-estructura-gestion-respuesta-id-tarea-wf

## MODIFIED Requirements

### Requirement: Master-Detail Loading Uses Shared AppLoadingState
El panel de detalle de Gestión Correspondencia MUST usar el componente shared `AppLoadingState` para el estado `loading`, eliminando cualquier temporización local (delay) en la vista.

#### Scenario: Loading uses AppLoadingState with delay
WHEN la ruta `/dashboard/gestion-correspondencia/respuesta/:id` está activa y la estructura está en estado `loading`  
THEN el panel MUST renderizar `AppLoadingState` (inline/card) con un delay configurado  
AND MUST NOT renderizar el contenido operativo (`GestionRespuesta`) mientras continúe `loading`.

#### Scenario: No local timing logic in consumer
WHEN `GestionCorrespondenciaRoute` implementa el estado `loading`  
THEN MUST NOT existir lógica de temporización (`setTimeout`/`clearTimeout`) en la vista para controlar la visibilidad del loader.

