# Servicios y reglas

- Ticket: DOC-56
- Cambio OpenSpec: doc-56-pruebas-integracion
- Clasificacion: cross_cutting

## Frontera web

`WebServiceImportarServicioWebModern.asmx.vb` publica: `ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`.

Cada método persistente evalúa gate, forma mínima del request, contexto confiable y composición. `ValidRequest` exige tarea positiva, operación, correlación y proveedor `INTEGRACIONSII`.

## Composición concreta

| Responsabilidad | Implementación |
|---|---|
| conexiones | `WorkflowModuleConnectionFactory`, `DocuarchiModuleConnectionFactory`, `RadicacionModuleConnectionFactory` |
| autorización | `ValidadorContextoImportacion` + `MySqlSiiImportAuthorizationRepository` |
| intención | `ServicioIntencionImportacion`, `MySqlImportIntentRepository`, `MySqlImportIntentConcurrencyGuard` |
| proveedor | `RegistroClientesProveedoresImportacion` + `SiiImportProvider` |
| tipología | `MySqlImportDocumentTypeResolver` |
| ejecución | `ImportServiceOrchestrator` + `ImportIntentStateMachine` |
| almacenamiento | `MySqlImportStorageMetadataRepository` + `LegacyImportDocumentStorageAdapter` |
| reconciliación | `ServicioReconciliacionImportacion` + `MySqlImportReconciliationRepository` + `ImportItemResultMapper` |
| telemetría | `MySqlExternalServiceTelemetryRepository` sobre DocuArchi |

## Pasos en el orden creado por `Compose`

1. `DownloadImportExecutionStep` → `RecursoObtenido`.
2. `PrepareImportExecutionStep` → `ExpedientePreparado`.
3. `PrepareImportIndicesExecutionStep` → `IndicesActualizados`.
4. `StoreImportExecutionStep` → `DocumentoAlmacenado`.
5. `CompleteImportExecutionStep(CacheActualizado)`.
6. `CompleteImportExecutionStep(Completada)`.

Cada transición pasa por `ImportIntentStateMachine.Intentar` y `MySqlImportIntentRepository.ActualizarTransicion`, que actualiza item e intención con versión optimista e inserta `workflow_import_intent_transition` dentro de la misma transacción.

## Proyección posterior a la escritura

El ASMX entrega el resultado del orquestador a `ServicioReconciliacionImportacion.ProjectExecutionResult`. Este vuelve a leer la evidencia persistida y aplica `ImportItemResultMapper`; solamente `Status="Disponible"` conserva identidad documental utilizable.
