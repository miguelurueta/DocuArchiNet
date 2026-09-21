# Secuencia de ejecución única programática

## Convención

- `Consumidor JS`: actor `<<external>>`; hoy no corresponde a un botón del modal.
- `Espera global`: elemento `<<conceptual>>`.
- `JS.*` y `VB.*`: símbolos validados.

## Diagrama

```mermaid
sequenceDiagram
    actor X as Consumidor JS <<external>>
    participant C as JS.Core.execute(request:Object):Promise
    participant W as Espera global <<conceptual>>
    participant E as VB.Endpoint.ExecuteImportIntent(request:ExecuteImportIntentRequestDto):ExecuteImportIntentResponseDto
    X->>C: execute(request)
    alt existe execution
        C-->>X: misma Promise
    else primera llamada
        C->>W: estado preparando → ejecutando
        C->>E: ExecuteImportIntent(request)
        alt respuesta exitosa
            E-->>C: ExecuteImportIntentResponseDto
            C-->>X: Snapshot(completado)
        else excepción
            E--xC: Error
            C-->>X: Promise rechazada + Snapshot(error)
        end
    end
```

## Fuentes

- `js/workflow/importar-servicio-web/importar-servicio-web-core.js`
- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`

Símbolos: `JS.Core.execute`, `VB.Endpoint.ExecuteImportIntent`.
