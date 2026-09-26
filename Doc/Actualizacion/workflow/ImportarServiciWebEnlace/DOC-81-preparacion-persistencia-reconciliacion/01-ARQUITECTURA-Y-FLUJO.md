# Arquitectura y flujo técnico

## Alcance revisado

Repositorio `DocuArchiNet`: frontera ASMX, servicios de preflight/intención/orquestación/reconciliación, proveedor SII, adaptadores de almacenamiento y repositorios MySQL modernos. La función legacy se reutiliza de forma aditiva; no se reescribió `AlmacenaDocumentoTareaWorkflow` ni se modificó el endpoint ASMX histórico para sus llamadores existentes.

## Recorrido Cliente → Controller → Service → Repository → Respuesta

```text
[Cliente Workflow: selección ENLASE]
  | PreflightImport(request: PreflightImportRequestDto)
  v
[Controller: WebServiceImportarServicioWebModern]
  | gate + ValidRequest + TryBuildImportContext
  | reconstruye recibo/código de barras desde servidor
  v
[ServicioPreflightImportacion.Preflight(contexto, request)]
  |-- selección vacía/duplicada/tipología no autorizada --> Error seguro, sin escritura
  |-- evidencia física válida ya importada -------------> DOCUMENT_ALREADY_IMPORTED
  `-- selección válida ----------------------------------> comandos + fingerprint
  |
  | CreateImportIntent(request)
  v
[ServicioIntencionImportacion.Crear]
  | huella = usuario+tarea+ruta+trámite+proveedor+capacidad+providerReference+items
  | URL temporal excluida
  v
[MySqlImportIntentRepository]
  | crea o reutiliza una intención y elementos
  |
  | ExecuteImportIntent(request)
  v
[ImportServiceOrchestrator.Execute]
  | adquiere IImportIntentConcurrencyGuard
  | recarga versión y revalida contexto persistido
  |-- contexto cambió --> código seguro, cero efectos
  `-- válido
       |
       v
  [DownloadImportExecutionStep.Ejecutar]
       | SiiImportProvider.DownloadAsync(capability=ANEXOS_RADICADO_ENLASE)
       | SiiExternalImportProviderClient.DownloadAnnexAsync
       | reconsulta consultarRadicado por referencia confiable
       | allowlist host + tamaño + tipo/contenido
       v
  [PrepareImportExecutionStep / PrepareImportIndicesExecutionStep]
       | archivo temporal y metadatos autorizados
       v
  [StoreImportExecutionStep.Ejecutar]
       | LegacyEnlaseImportDocumentStorageAdapter.Almacenar
       | valida sesión ID_TAREA_SELECCIONDA_ENLACE/ruta
       | invoca una vez PreAlmacenaDocumentoAnexosEnlaceIntegracionSII
       | entrega RutaArchivoPreparada; legacy no vuelve a descargar
       v
  [VerifyStoredImportExecutionStep.Ejecutar]
       | MySqlImportReconciliationRepository.ObtenerItem
       |-- 1 documento + 1 relación misma tarea + 1 registro físico --> Reconciliada
       |-- registro lógico sin físico -------------------------------> Recuperable
       |-- duplicidad/discrepancia/no verificable -------------------> Incierto
       v
  [CompleteImportExecutionStep(Completada)]
       |
       v
[ServicioReconciliacionImportacion.ProjectExecutionResult]
  | oculta ID/nombre/tipo salvo evidencia confirmada
  | agrega Completado / Parcial / Recuperable / ResultadoIncierto
  v
[Respuesta ASMX: ExecuteImportIntentResponseDto]
```

## Decisiones de composición

`WebServiceImportarServicioWebModern.Compose` selecciona `LegacyEnlaseImportDocumentStorageAdapter`, `EnlaseImportEffectConfigurationRepository`, coordinadores de expediente nulos y el paso de verificación solo cuando la capacidad confiable es `ANEXOS_RADICADO_ENLASE`. Para constancias conserva `LegacyImportDocumentStorageAdapter`, coordinadores DOC-67 y finalización por `CacheActualizado`.

Los endpoints ASMX devuelven envelopes y códigos funcionales; no se documentan códigos HTTP distintos porque esa frontera normalmente responde HTTP 200. Fallos de sesión/gate/contexto se expresan en `ErrorImportacionServicioDto.Codigo`.