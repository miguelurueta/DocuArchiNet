# Arquitectura y secuencia

```mermaid
sequenceDiagram
    participant C as EXT: Cliente
    participant A as CODE: WebServiceImportarServicioWebModern
    participant P as CODE: ServicioPreflightImportacion
    participant T as CODE: IImportDocumentTypeResolver
    participant R as CODE: IImportEffectConfigurationRepository
    participant B as CODE: ImportEffectPlanBuilder
    C->>A: PreflightImport(request)
    A->>P: Preflight(contexto, request)
    P->>T: Resolver(contexto, tipoId, nombre)
    P->>R: Obtener(contexto)
    R-->>P: ImportEffectConfiguration
    P->>B: Build(contexto, items, configuración)
    B-->>P: IList(Of ImportEffectPlanDto)
    P-->>C: respuesta + Executable + fingerprint
```

`CreateImportIntent` recompone el mismo preflight y compara huella y requisitos antes de `_guard.Adquirir`. El plan es lógico; `ImportServiceOrchestrator` conserva la única ruta que materializa efectos DOC-67.
