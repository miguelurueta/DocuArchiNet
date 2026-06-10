# AppUploadDocumental - Diagrama de clases

## Proposito

Representar el contrato del adaptador documental, sus tipos de metadata y las capas de servicio/utilidad que participan en la migracion desde `FileUploadHandler.js`.

```mermaid
classDiagram
  direction LR

  class AppUploadDocumental
  class AppUpload
  class AppProgressBatch
  class UploadDocumentalContext
  class UploadDocumentalFileMetadata
  class UploadDocumentalQueueItem
  class UploadConfig
  class TipoDocumentalOption
  class almacenamientoDocumentalUploadService
  class uploadConfigService
  class tipoDocumentalService
  class tipoDocumentalSuggestionUtils
  class storageFileUtils

  AppUploadDocumental --> AppUpload : composes
  AppUploadDocumental --> AppProgressBatch : uses for batch
  AppUploadDocumental --> UploadDocumentalContext : receives
  AppUploadDocumental --> UploadDocumentalFileMetadata : owns by uid
  AppUploadDocumental --> UploadConfig : loads
  AppUploadDocumental --> TipoDocumentalOption : loads
  AppUploadDocumental --> UploadDocumentalQueueItem : builds
  AppUploadDocumental --> uploadConfigService : calls
  AppUploadDocumental --> tipoDocumentalService : calls
  AppUploadDocumental --> almacenamientoDocumentalUploadService : calls
  AppUploadDocumental --> tipoDocumentalSuggestionUtils : calls
  AppUploadDocumental --> storageFileUtils : calls
  UploadDocumentalQueueItem --> AppUploadFile : wraps
  UploadDocumentalQueueItem --> UploadDocumentalFileMetadata : wraps
```

## Lectura

- `AppUploadDocumental` es el adaptador de negocio, no el motor visual base.
- `AppUpload` conserva la responsabilidad de seleccion y lista de archivos.
- `AppProgressBatch` conserva la responsabilidad de proceso secuencial y cancelacion.
- La API nueva queda encapsulada en servicios, no en el componente visual.
