# Contrato y mapping

- Ticket: DOC-54
- Cambio OpenSpec: doc-54-reconciliacion-lista-documetos
- Clasificacion: cross_cutting

`ImportItemResultDto` conserva SchemaVersion 1.0 y añade `ReachedPhase`, `TaskId`, `DocumentName` y `ContentType`. Junto con `DocumentId`, `PersistenceKnown`, `Retryable`, códigos seguros y correlación permite refrescar la lista sin depender de `dato_lista`.

El mapper asigna `Disponible` exclusivamente a evidencia confirmada y relación única. Fases parciales, detenidas y fallidas tienen salidas explícitas. Persistencia desconocida o `ResultadoIncierto` producen `Verificando`/`ResultadoIncierto`. Cualquier resultado no disponible oculta identificador, nombre y tipo del documento.
