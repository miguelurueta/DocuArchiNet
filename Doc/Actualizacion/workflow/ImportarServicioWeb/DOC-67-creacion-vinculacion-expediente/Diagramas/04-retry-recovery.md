# Detención, retry y recovery

Fuentes: `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`, `Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb`.

Referencias CODE: `ImportServiceOrchestrator.Execute(ContextoImportacionServicio,ExecuteImportIntentRequestDto):ExecuteImportIntentResponseDto`.

```mermaid
stateDiagram-v2
  [*] --> Preparada
  Preparada --> Detenida: StopRequested
  Detenida --> EnEjecucion: versión vigente
  EnEjecucion --> Parcial: efecto conocido fallido
  EnEjecucion --> ResultadoIncierto: postcondición no demostrable
  EnEjecucion --> Reconciliada: efectos confirmados
  Reconciliada --> Completada
  Completada --> Completada: recovery de solo lectura
```
