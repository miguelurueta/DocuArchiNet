# Secuencia de apertura y consulta

## Convención

- Participantes `JS.*`/`VB.*`: funciones o métodos reales.
- `Usuario`: actor `<<external>>` excluido de resolución.
- Los tipos son formas documentales JavaScript o tipos .NET exactos.

## Diagrama

```mermaid
sequenceDiagram
    actor U as Usuario <<external>>
    participant UI as JS.Ui.open(control:Object,event:Event):Boolean
    participant C as JS.Core.open(providerId:String):Promise
    participant R as JS.Registry.resolve(providerId:any):Resolution
    participant Q as JS.Core.query(request:Object):Promise
    participant A as JS.Ui.createBackendAdapter(api:Object,control:Object):Adapter
    participant RC as VB.Endpoint.ResolveCapabilities(request:ResolveCapabilitiesRequestDto):ResolveCapabilitiesResponseDto
    participant QI as VB.Endpoint.QueryItems(request:QueryItemsRequestDto):QueryItemsResponseDto
    U->>UI: clic ctw-document-action-service
    UI->>C: open(providerId)
    C->>R: resolve(providerId)
    alt proveedor no resuelto
        R-->>C: PROVIDER_NOT_CONFIGURED/NOT_MIGRATED/NOT_SUPPORTED
        C-->>UI: Snapshot(error)
    else proveedor resuelto
        C->>Q: query({})
        Q->>A: queryItems(request)
        A->>RC: ResolveCapabilities(request)
        alt Error no nulo
            RC-->>A: Error.Codigo
            A-->>Q: Promise rechazada
        else capacidades disponibles
            A->>QI: QueryItems(request)
            QI-->>A: QueryItemsResponseDto
            A-->>Q: response.Items
            Q-->>UI: Snapshot(resultados|vacio)
        end
    end
```

## Fuentes

- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`
- `js/workflow/importar-servicio-web/importar-servicio-web-core.js`
- `js/workflow/importar-servicio-web/importar-servicio-web-provider-registry.js`
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`

Símbolos: `JS.Ui.open`, `JS.Core.open`, `JS.Registry.resolve`, `JS.Core.query`, `JS.Ui.createBackendAdapter`, `VB.Endpoint.ResolveCapabilities`, `VB.Endpoint.QueryItems`.
