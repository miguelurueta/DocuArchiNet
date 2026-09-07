# Contrato y mapping

- Ticket: DOC-53
- Cambio OpenSpec: doc-53-orquestacion-estados
- Clasificacion: cross_cutting

Execute recibe `IntentId`, `VersionToken` y la solicitud cooperativa `StopRequested`. Execute y Get retornan items con `Status`, `DocumentId`, `PersistenceKnown`, `Retryable`, `ErrorCode`, mensaje seguro y `CorrelationId`. El contrato conserva `SchemaVersion = 1.0` y defaults compatibles.

El comando moderno de almacenamiento contiene ruta temporal, gabinete, radicado, ruta/tarea workflow, tipología y caso. El adaptador traduce ese comando a la firma legacy sin exponer `ClassAlmacenamiento` al orquestador.
