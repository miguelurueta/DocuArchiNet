# Validaciones y errores del borde ASMX

## Convención

- `VB.*`: método real validado por reflexión, incluida sobrecarga exacta.
- Nodos de decisión: condiciones implementadas.
- Códigos de error: valores funcionales devueltos por DTO.

## Diagrama

```mermaid
flowchart TD
    START["Solicitud POST ASMX"] --> GATE["VB.Endpoint.FeatureEnabled(): System.Boolean"]
    GATE -->|false| DISABLED["FEATURE_DISABLED"]
    GATE -->|true| VALID["VB.Endpoint.ValidRequest(request:SolicitudImportacionServicioDto): System.Boolean"]
    VALID -->|false| INVALID["INVALID_REQUEST"]
    VALID -->|true| CONTEXT["VB.Endpoint.TryBuildImportContext(request,context ByRef,session ByRef,failureCode ByRef): System.Boolean"]
    CONTEXT -->|false| SAFE["Código seguro de sesión/tarea/contexto"]
    CONTEXT -->|true| PROVIDER["VB.Endpoint.ResolveProvider(providerId:String,attempts:IExternalServiceAttemptRecorder): ResultadoResolucionClienteProveedorImportacion"]
    PROVIDER -->|no encontrado| UNAVAILABLE["Código seguro de configuración/proveedor"]
    PROVIDER -->|encontrado| OP["Operación específica"]
```

## Fuentes

- `webservice/WebServiceImportarServicioWebModern.asmx.vb`
- `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`

Símbolos: `VB.Endpoint.FeatureEnabled`, `VB.Endpoint.ValidRequest`, `VB.Endpoint.TryBuildImportContext`, `VB.Endpoint.ResolveProvider`.
