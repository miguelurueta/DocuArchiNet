# Frontera y endpoints

Fuentes: `webservice/WebServiceImportarServicioWebModern.asmx.vb`.

Referencias CODE: `WebServiceImportarServicioWebModern.ExecuteImportIntent(ExecuteImportIntentRequestDto):ExecuteImportIntentResponseDto`; `ImportServiceOrchestrator.Execute(ContextoImportacionServicio,ExecuteImportIntentRequestDto):ExecuteImportIntentResponseDto`.

```mermaid
sequenceDiagram
  actor U as EXT:Usuario Workflow
  participant W as CODE:WebServiceImportarServicioWebModern
  participant O as CODE:ImportServiceOrchestrator
  U->>W: ExecuteImportIntent(ExecuteImportIntentRequestDto)
  alt gate/contexto/request inválido
    W-->>U: ExecuteImportIntentResponseDto.Error
  else autorizado
    W->>O: Execute(ContextoImportacionServicio, request)
    O-->>W: ExecuteImportIntentResponseDto
    W-->>U: ExecuteImportIntentResponseDto
  end
```
