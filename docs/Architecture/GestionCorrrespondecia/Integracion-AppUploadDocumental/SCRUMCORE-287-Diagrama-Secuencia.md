# SCRUMCORE-287 - Diagrama De Secuencia

## Carga De Configuracion Upload

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as GestionRespuestaUploadDocumental
    participant App as AppUploadDocumental
    participant Loader as loadGestionRespuestaUploadConfig
    participant Service as getConfiguracionUploadCorrespondencia
    participant API as DocuArchiApi

    Usuario->>UI: Abre modal Adjuntar documentos
    UI->>App: Renderiza AppUploadDocumental
    App->>Loader: Solicita configuracion documental
    Loader->>Service: getConfiguracionUploadCorrespondencia()
    Service->>API: GET /configuracion-upload?nameProceso=CORRESPO
    API-->>Service: success/data
    Service->>Service: Normaliza ExtensionUpload y LengUpload
    Service-->>Loader: ConfiguracionUploadCorrespondencia
    Loader-->>App: UploadDocumentalConfig
    App-->>UI: Upload habilitado con accept/maxSizeBytes
```

## Guardar Un Archivo

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as AppUploadBatchView
    participant Actions as useAppUploadDocumentalActions
    participant Storage as almacenamientoDocumentalUpload.service
    participant API as DocuArchiApi
    participant Context as GestionRespuestaDocumentosContext
    participant Table as AppTreeTable

    Usuario->>UI: Click Guardar archivo
    UI->>Actions: saveOne(uid)
    Actions->>Actions: Valida extension, tamano, tipologia y fecha
    Actions->>Storage: uploadAndStoreOneDocument(file, request)
    Storage->>API: POST /upload-temporal/init
    API-->>Storage: rutaTemporalId, archivoTemporalId, chunkSizeBytes

    loop Por cada chunk
        Storage->>API: PUT /chunk/{chunkIndex}
        alt Timeout o ERR_NETWORK sin response
            Storage->>Storage: Retry controlado con backoff
            Storage->>API: PUT /chunk/{chunkIndex}
        end
        API-->>Storage: OK
    end

    Storage->>API: GET /status
    API-->>Storage: Estado temporal
    Storage->>API: POST /complete
    API-->>Storage: OK
    Storage->>API: POST /almacenamiento
    API-->>Storage: IdAlmacen / AnexoRespuesta
    Storage-->>Actions: StoredResult
    Actions-->>UI: Remueve archivo almacenado de cola
    Actions-->>Context: onStored(source=single, remainingFiles)
    Context->>Table: refreshDocumentos()

    alt remainingFiles == 0
        Actions-->>UI: Cierra modal
    else Hay archivos pendientes
        Actions-->>UI: Mantiene modal abierto
    end
```

## Guardar Todo Con Archivo Grande Y Pendientes

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as AppUploadBatchView
    participant Actions as useAppUploadDocumentalActions
    participant Storage as almacenamientoDocumentalUpload.service
    participant API as DocuArchiApi
    participant Context as GestionRespuestaDocumentosContext

    Usuario->>UI: Click Guardar todo
    UI->>Actions: saveAll()
    Actions->>Actions: Prevalida todos los archivos

    alt Archivo sin tipologia
        Actions->>UI: Marca fila en error
        Actions->>Actions: No procesa ese archivo
    end

    loop Archivos validos
        Actions->>Storage: uploadAndStoreOneDocument(file)
        Storage->>API: init + chunks + status + complete + almacenamiento
        API-->>Storage: StoredResult
        Storage-->>Actions: StoredResult
        Actions->>UI: Remueve archivo almacenado de cola
    end

    Actions->>Actions: Calcula remainingFiles
    Actions-->>Context: onBatchComplete(summary)
    Context->>Context: refreshDocumentos si stored > 0

    alt remainingFiles == 0 y sin errores/cancelaciones
        Context-->>UI: Cierra modal
    else Quedan pendientes, errores o cancelados
        Context-->>UI: Mantiene modal abierto
    end
```

## Cancelacion Global

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as AppUploadBatchView
    participant Actions as useAppUploadDocumentalActions
    participant Storage as almacenamientoDocumentalUpload.service
    participant API as DocuArchiApi

    Usuario->>UI: Click Cancelar carga
    UI->>Actions: cancelAll()
    Actions->>Actions: Marca globalCancelRequested=true
    Actions->>Actions: abort() de controllers activos
    Storage-->>Actions: storage_aborted / AbortError
    Actions->>UI: Archivo activo queda cancelled
    Actions->>UI: Archivos no iniciados quedan en cola
    Actions-->>UI: onBatchComplete(cancelled > 0, remainingFiles > 0)
    UI-->>Usuario: Modal permanece abierto

    opt Limpieza temporal cuando aplica
        Storage->>API: DELETE /upload-temporal/{rutaTemporalId}/{archivoTemporalId}
        API-->>Storage: OK o error controlado
    end
```

## Cancelacion Unitaria

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as AppUploadBatchView
    participant Actions as useAppUploadDocumentalActions
    participant Storage as almacenamientoDocumentalUpload.service

    Usuario->>UI: Click Cancelar archivo
    UI->>Actions: cancelFile(uid)
    Actions->>Actions: Busca AbortController por uid
    Actions->>Actions: controller.abort()
    Storage-->>Actions: storage_aborted / AbortError
    Actions->>UI: Fila queda en estado cancelled
    UI-->>Usuario: Boton Guardar todo queda disponible para reintentar
```

## Manejo De Doble Click En Guardar Archivo

```mermaid
sequenceDiagram
    actor Usuario
    participant UI as AppUploadBatchView
    participant Actions as useAppUploadDocumentalActions

    Usuario->>UI: Doble click Guardar archivo
    UI->>Actions: saveOne(uid)
    Actions->>Actions: Registra AbortController activo
    UI->>Actions: saveOne(uid) segundo disparo
    Actions->>Actions: Detecta controller activo para uid
    Actions-->>UI: Ignora segundo disparo
```
