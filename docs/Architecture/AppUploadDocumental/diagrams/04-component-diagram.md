# AppUploadDocumental - Diagrama de componentes

## Proposito

Mostrar la separacion entre UI, adaptador documental, servicios HTTP y utilidades puras.

```mermaid
flowchart TB
  subgraph Consumer["Modulo consumidor"]
    Screen["Pantalla o formulario documental"]
    Refresh["Refrescar tablas, visores o listados"]
  end

  subgraph Component["AppUploadDocumental"]
    Container["Container documental"]
    MetadataState["metadata por archivo"]
    Actions["acciones guardar cancelar"]
    TipoSelector["selector tipologia por archivo"]
    DateInput["fecha por archivo"]
  end

  subgraph SharedUI["Componentes shared"]
    Upload["AppUpload"]
    Batch["AppProgressBatch"]
    Loading["AppLoadingState feedback"]
  end

  subgraph Services["Servicios frontend"]
    ConfigService["uploadConfig.service"]
    TipoService["tipoDocumental.service"]
    StorageService["almacenamientoDocumentalUpload.service"]
  end

  subgraph Utils["Utilidades puras"]
    FileUtils["storageFile.utils"]
    SuggestUtils["tipoDocumentalSuggestion.utils"]
  end

  subgraph Backend["APIs backend"]
    ConfigApi["API configuracion upload"]
    TipoApi["API tipologias"]
    Init["POST init temporal"]
    Chunk["PUT chunk"]
    Complete["POST complete"]
    Cancel["DELETE upload temporal"]
    Store["POST almacenamiento"]
  end

  Screen --> Container
  Container --> Upload
  Container --> Batch
  Container --> MetadataState
  Container --> Actions
  Container --> TipoSelector
  Container --> DateInput
  Container --> Loading

  Container --> ConfigService
  Container --> TipoService
  Actions --> StorageService

  ConfigService --> ConfigApi
  TipoService --> TipoApi
  StorageService --> Init
  StorageService --> Chunk
  StorageService --> Complete
  StorageService --> Cancel
  StorageService --> Store

  Container --> FileUtils
  Container --> SuggestUtils
  StorageService --> FileUtils

  Container --> Refresh
```

## Principios

- El componente no llama endpoints directamente; usa servicios.
- El modulo consumidor decide donde se renderiza el componente y como refrescar datos.
- Las reglas testeables viven fuera del JSX.
- `AppUploadDocumental` no reemplaza `AppUpload`; lo especializa para negocio documental.
