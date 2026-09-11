# 05 — Máquina de estados y reintento

Fuente: `ImportIntentStateMachine.CrearTabla` e `ImportServiceOrchestrator.EsReintentoSeguro`.

```mermaid
stateDiagram-v2
    [*] --> Creada
    Creada --> Validada
    Creada --> FallidaAntesDePersistir
    Creada --> Detenida

    Validada --> RecursoObtenido
    Validada --> FallidaAntesDePersistir
    Validada --> ResultadoIncierto
    Validada --> Detenida

    RecursoObtenido --> ExpedientePreparado
    RecursoObtenido --> Validada: reintento seguro
    RecursoObtenido --> Parcial
    RecursoObtenido --> ResultadoIncierto
    RecursoObtenido --> Detenida

    ExpedientePreparado --> IndicesActualizados
    ExpedientePreparado --> Validada: reintento seguro
    ExpedientePreparado --> Parcial
    ExpedientePreparado --> ResultadoIncierto
    ExpedientePreparado --> Detenida

    IndicesActualizados --> DocumentoAlmacenado
    IndicesActualizados --> Validada: reintento seguro
    IndicesActualizados --> Parcial
    IndicesActualizados --> ResultadoIncierto
    IndicesActualizados --> Detenida

    DocumentoAlmacenado --> CacheActualizado
    DocumentoAlmacenado --> Parcial
    DocumentoAlmacenado --> ResultadoIncierto
    DocumentoAlmacenado --> Detenida
    CacheActualizado --> Completada
    CacheActualizado --> Parcial

    ResultadoIncierto --> RequiereDecision
    ResultadoIncierto --> Reconciliada
    RequiereDecision --> Reconciliada
    Reconciliada --> Completada
    Reconciliada --> Parcial

    FallidaAntesDePersistir --> Validada: persistencia conocida, sin documento
    Parcial --> Validada: persistencia conocida, sin documento
    Detenida --> Validada: persistencia conocida, sin documento
```

`EsReintentoSeguro` rechaza cualquier item con persistencia desconocida o `IdDocumento` ya asignado. `ResultadoIncierto` no se reintenta automáticamente.
