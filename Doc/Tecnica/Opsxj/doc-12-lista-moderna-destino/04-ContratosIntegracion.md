# LISTA-MODERNA-DESTINO

- Ticket: DOC-12
- Cambio OpenSpec: doc-12-lista-moderna-destino
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

El endpoint de lectura es `../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea`, invocado por `POST` JSON con `{ "idTarea": 41 }` y `credentials: "same-origin"`. La respuesta ASMX se desempaqueta desde `d` y se normaliza desde `PrevisualizacionTransicionDto`: `IdTarea`, `TipoDecision`, `Contexto.Radicado`, `Contexto.GrupoActual`, `Destinos`, `TokenVersion` y `Error`.

Cada destino usa `Id`, `Nombre`, `Destinatario`, `Grupo`, `Tipo` y `Orden`. DOC-12 omite trámite y actividad actual legible porque no están publicados. Al seleccionar se publica `workflow:destination-selected` con `idTarea`, `idConector`, `tokenVersion` y resumen visible. El contrato sirve a una confirmación posterior y no cambia el esquema ni invoca el endpoint de ejecución.
