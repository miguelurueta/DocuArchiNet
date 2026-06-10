# AppProgressBatch - Diagrama de secuencia

## Proposito

Describir el flujo principal de ejecucion batch con item exitoso, advertencia, error controlado, cancelacion y cierre.

```mermaid
sequenceDiagram
  autonumber
  actor User as Usuario
  participant Consumer as Componente consumidor
  participant Batch as AppProgressBatch
  participant Process as processItem
  participant Abort as AbortController

  Consumer->>Batch: abre con items y processItem
  User->>Batch: iniciar proceso
  Batch->>Batch: validar items

  alt items vacios
    Batch-->>User: mostrar emptyMessage
    Batch-->>Consumer: onComplete sin items
  else items disponibles
    Batch->>Abort: crear controller
    loop por cada item
      Batch->>Process: process item
      Process-->>Batch: actualiza label progreso y fase

      alt success
        Process-->>Batch: resultado success
        Batch->>Batch: incrementar success y avanzar
      else warning
        Process-->>Batch: resultado warning
        Batch->>Batch: registrar advertencia y avanzar
      else skipped
        Process-->>Batch: resultado skipped
        Batch->>Batch: registrar omitido y avanzar
      else controlled error
        Process-->>Batch: resultado controlled error
        Batch-->>User: pedir continuar o cancelar
        alt usuario continua
          User->>Batch: continuar
          Batch->>Batch: registrar error controlado y avanzar
        else usuario cancela
          User->>Batch: cancelar
          Batch->>Abort: abort
          Batch-->>Consumer: onCancel
        end
      else fatal error
        Process-->>Batch: resultado fatal error
        Batch-->>Consumer: onError
        Batch->>Batch: detener proceso
      end
    end

    Batch-->>Consumer: onComplete
    Batch-->>User: mostrar resumen
  end
```

## Observaciones

- `setCurrentLabel`, `setItemProgress` y `setPhase` son llamadas desde el proceso consumidor hacia el contexto.
- Cancelar no depende del dominio; se propaga por `AbortSignal`.
- El resumen siempre debe reflejar la ruta real: exito, cancelacion, advertencias, omitidos o error.
