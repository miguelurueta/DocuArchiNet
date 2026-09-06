# Cancelación y timeout

```mermaid
flowchart TD
  IO[OperationCanceledException] --> C{Token externo cancelado}
  C -->|Sí| Cancel[EXTERNAL_CANCELLED]
  C -->|No| Timeout[EXTERNAL_TIMEOUT]
```
