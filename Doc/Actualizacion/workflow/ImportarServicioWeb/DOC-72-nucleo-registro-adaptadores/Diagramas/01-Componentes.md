# Componentes DOC-72

## Convención

- Nodos `JS.*` y `VB.*`: símbolos de código validados automáticamente.
- `EXT_*`: `<<external>>`, no se resuelve contra código.
- `CONCEPT_*`: `<<conceptual>>`, no representa una clase.

## Diagrama

```mermaid
flowchart TD
    EXT_USER["Usuario <<external>>"] --> UI["JS.Ui.initialize(options:Object): Control|null"]
    UI --> CORE["JS.Core.create(options:Object): Core"]
    CORE --> REG["JS.Registry.create(options:Object): Registry"]
    UI --> API["JS.Api.create(options:Object): ApiClient"]
    API --> ASMX["WebServiceImportarServicioWebModern"]
    VB["VB.Webworkflow.RegisterImportarServicioWebModernAssets(): System.Void"] --> UI
    ASMX --> EXT_PROVIDER["Proveedor externo <<external>>"]
```

## Fuentes

- `js/workflow/importar-servicio-web/importar-servicio-web-{api,provider-registry,core,ui}.js`
- `workflow/Webworkflow.aspx.vb`
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`

Símbolos: `JS.Api.create`, `JS.Registry.create`, `JS.Core.create`, `JS.Ui.initialize`, `VB.Webworkflow.RegisterImportarServicioWebModernAssets`.
