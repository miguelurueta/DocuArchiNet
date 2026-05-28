# AppUploadDocumental - Diagrama de secuencia

## Proposito

Describir el flujo completo: configuracion, seleccion, metadata por archivo, upload por chunks, registro final y refresco del consumidor.

```mermaid
sequenceDiagram
  autonumber
  actor User as Usuario
  participant Module as Modulo consumidor
  participant DocUpload as AppUploadDocumental
  participant AppUpload as AppUpload
  participant Config as uploadConfig.service
  participant Tipos as tipoDocumental.service
  participant Suggest as tipoDocumentalSuggestion.utils
  participant Batch as AppProgressBatch
  participant Storage as almacenamientoDocumentalUpload.service
  participant Api as API almacenamiento

  Module->>DocUpload: render componente
  DocUpload->>Config: obtener configuracion
  Config-->>DocUpload: accept, maxSize, rules
  DocUpload->>Tipos: obtener tipologias
  Tipos-->>DocUpload: opciones de tipologia
  DocUpload->>AppUpload: configurar validacion y drag

  User->>AppUpload: selecciona archivos
  AppUpload-->>DocUpload: onChange(files)
  loop cada archivo nuevo
    DocUpload->>DocUpload: validar extension/tamano
    DocUpload->>Suggest: sugerir tipologia por nombre
    Suggest-->>DocUpload: tipo sugerido
    DocUpload->>DocUpload: crear metadata por uid
  end

  User->>DocUpload: ajusta tipologia/fecha por archivo

  alt guardar archivo individual
    User->>DocUpload: guardar archivo
    DocUpload->>Storage: subir archivo documental
  else guardar lote
    User->>DocUpload: guardar lote
    DocUpload->>Batch: open items=listos
    Batch->>Storage: procesar archivo
  end

  Storage->>Api: POST init temporal
  Api-->>Storage: rutaTemporalId, archivoTemporalId, chunkSizeBytes

  loop chunks
    Storage->>Api: PUT chunk bytes
    Api-->>Storage: OK chunkIndex
    Storage-->>DocUpload: progreso
  end

  Storage->>Api: POST complete
  Api-->>Storage: Estado Completed

  Storage->>Api: POST almacenamiento documento individual
  Api-->>Storage: AlmacenarDocumentoResponse
  Storage-->>DocUpload: stored result
  DocUpload-->>Module: onStored

  alt lote finalizado
    Batch-->>DocUpload: onComplete
    DocUpload-->>Module: onBatchComplete
    Module->>Module: refrescar listado/documentos
  end

  alt cancelacion durante upload
    User->>DocUpload: cancelar
    DocUpload->>Storage: abort y cancelar temporal
    Storage->>Api: DELETE upload temporal
    Api-->>Storage: Cancelled
    Storage-->>DocUpload: cancelled
  end
```

## Puntos criticos

- La configuracion se carga antes de habilitar seleccion.
- La tipologia es metadata por archivo, no global.
- El POST final se ejecuta una vez por archivo para respetar tipologia individual sin cambiar backend.
- `onStored` y `onBatchComplete` reemplazan callbacks globales legacy.
