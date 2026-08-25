# Contratos y estado de cliente

DOC-33 consume sin ampliarlos los endpoints JSON autenticados de DOC-32. El cliente no envía actividad de destino, usuario, grupo, Ruta, Flujo, contexto ni datos de infraestructura; esas identidades se reconstruyen y validan en el servidor.

| Operación | URL relativa | Payload | Resultado que consume la UI |
| --- | --- | --- | --- |
| Preview | `../webservice/WebServiceWorkflowModern.asmx/PreviewDevolverActividad` | `{ idTarea, termino, cursor, tamanoPagina }` | `TokenVersion`, contexto resumido, destinos, cursor siguiente, `HayMas` o bloqueo funcional. |
| Ejecución | `../webservice/WebServiceWorkflowModern.asmx/EjecutarDevolverActividad` | `{ idTarea, idConector, tokenVersion }` | `Exito`, `EstadoFinal`, mensaje funcional, código de bloqueo, reintentabilidad y advertencias saneadas. |

El adaptador de preview desempaqueta la respuesta ASMX (`d`) y conserva por destino únicamente `IdConector`, `NombreActividad`, destinatario o grupo resumido, tipo de contexto y orden. No deriva identificadores de Ruta, Flujo ni identidad de usuario desde campos no publicados.

## Estado aislado

El estado de `WorkflowReturnActivityUi` contiene consulta, cursor, historial de páginas, selección, número de solicitud y `AbortController`. Toda búsqueda nueva invalida la selección y emite el evento propio `workflow:return-activity-invalidated`; las respuestas con un número anterior se descartan. La selección autorizada emite `workflow:return-activity-selected` con la terna necesaria para la confirmación, sin mezclar eventos de otras transiciones.

La búsqueda se demora 300 ms y solo se envía desde dos caracteres. La página inicial puede pedir destinos sin texto; la continuación usa exclusivamente el cursor recibido. Esto limita solicitudes accidentales y evita aplicar una respuesta anterior sobre una consulta más reciente.
