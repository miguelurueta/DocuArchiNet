# Resumen técnico

- Ticket: DOC-56
- Cambio OpenSpec: doc-56-pruebas-integracion
- Clasificacion: cross_cutting

## Objetivo implementado

Importar una imagen de inscripción devuelta por SII hacia la tarea Workflow seleccionada, persistir una intención idempotente, ejecutar fases auditables, delegar el almacenamiento físico a `ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow` y devolver al consumidor un documento solamente cuando gabinete y relación con la tarea hayan sido confirmados.

## Entradas públicas

Todas las solicitudes heredan `SolicitudImportacionServicioDto`: `SchemaVersion`, `OperationId`, `CorrelationId`, `TaskId` y `ProviderId`. Según la operación, el frontend agrega:

- consulta: `CodigoBarras`, `ContinuationToken`, `PageSize`;
- preview: `ExternalKey`;
- preflight: `Items[]`;
- creación: `IdempotencyKey`, `Items[]`, `Requirements[]`, `ContextFingerprint`, `Radicado`;
- ejecución: `IntentId`, `VersionToken`, `StopRequested`;
- consulta/reconciliación: `IntentId` y, para el elemento focal, `ExternalKey`.

## Autoridad del servidor

`WebServiceImportarServicioWebModern.TryBuildImportContext` contrasta el request con usuario, grupo, login, tarea seleccionada, ruta y trámite de la sesión. `MySqlSiiImportAuthorizationRepository` exige `ADJUNTAR_IMAGENES_PREDETERMINADA`, tarea activa, selección vigente y coincidencia de ruta.

El navegador no controla credenciales SII, URL de descarga, recibo SII, gabinete, ruta física, clase documental, ID contextual de lista de chequeo ni usuario de almacenamiento.

## Fronteras preservadas

- `workflow/ClassAlmacenamiento.vb` sigue siendo caja negra legacy.
- `LegacyImportDocumentStorageAdapter.vb` contiene la única llamada moderna a `AlmacenaDocumentoTareaWorkflow`.
- Los ASMX históricos y `JSProgresBar.js` no inician la ruta moderna.
- El gate se entrega con `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.
