# AppProgressBatch - Diagrama de clases

## Proposito

Representar el contrato publico, los tipos de estado y las responsabilidades internas del componente reusable `AppProgressBatch`.

```mermaid
classDiagram
  direction LR

  class AppProgressBatch
  class AppProgressBatchItemContext
  class AppProgressBatchItemResult
  class AppProgressBatchSummary
  class BatchLifecycleState
  class BatchProgressState
  class AbortController
  class ConsumerProcess

  AppProgressBatch --> AppProgressBatchItemContext : creates
  AppProgressBatch --> AppProgressBatchSummary : emits
  AppProgressBatch --> BatchProgressState : owns
  AppProgressBatch --> AbortController : owns per run
  AppProgressBatch --> ConsumerProcess : calls
  ConsumerProcess --> AppProgressBatchItemResult : returns
  BatchProgressState --> BatchLifecycleState : uses
  AppProgressBatchItemContext --> AbortController : exposes signal
```

## Lectura

- `AppProgressBatch` no conoce dominio; solo coordina items y resultados.
- El consumidor inyecta la operacion concreta en `processItem`.
- `AbortController` permite cancelar sin depender de banderas globales.
- Los resultados tipados reemplazan `YES`, `CTRL`, `CTRLRETURN` y errores string del legacy.
