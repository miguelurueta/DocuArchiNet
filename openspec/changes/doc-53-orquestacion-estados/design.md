## Context

DOC-53 mueve al servidor la coreografía de intenciones DOC-52 y consume el cliente DOC-51. El flujo actual reparte efectos secuenciales entre expediente, índices, almacenamiento y caché sin transacción global.

## Goals / Non-Goals

**Goals:** ejecutar y consultar estado por item; aislar almacenamiento legacy; clasificar fallo, detención e incertidumbre.

**Non-Goals:** sustituir ASMX/JS legacy, paralelizar, modificar `ClassAlmacenamiento`, implementar reconciliación completa o prometer transacción distribuida.

## Decisions

### D-01 — Orquestador único
`ImportServiceOrchestrator` revalida contexto, carga la intención y coordina puertos; ningún archivo legacy adquiere responsabilidades nuevas.

### D-02 — Secuencia y versión
Los items avanzan uno por uno. Cada transición persiste fecha UTC y nuevo `VersionToken`; una versión obsoleta falla sin efectos.

### D-03 — Máquina pura
`ImportIntentStateMachine` declara transiciones sin SQL, HTTP, sesión o storage. Aceptaciones y rechazos se auditan mediante un puerto saneado.

### D-04 — Orden y límites locales
El pipeline es `Validada → RecursoObtenido → ExpedientePreparado → IndicesActualizados → DocumentoAlmacenado → CacheActualizado → Completada`. Cada fase confirma su propio efecto local.

### D-05 — Storage inmutable
`ImportDocumentStoragePort` normaliza el comando y `LegacyImportDocumentStorageAdapter` construye los 16 argumentos de `AlmacenaDocumentoTareaWorkflow`, mapeando `YES`, error y excepción.

### D-06 — Clasificación conservadora
Antes de efectos se usa `FallidaAntesDePersistir`. Si un efecto pudo ocurrir sin confirmación se persiste `ResultadoIncierto`, sin compensación ni reintento automático.

### D-07 — Detención cooperativa
La detención se consulta entre fases/items, no interrumpe una mutación, no revierte confirmados y marca pendientes `Detenida`.

### D-08 — Contrato consultable v1
Modelos/DTOs incluyen `PersistenceKnown`, `Retryable`, correlación y versión. Get lee exclusivamente la instantánea persistida.

### D-09 — Aislamiento y rollback
Solo se agregan componentes modernos, pruebas, proyecto y `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-53-orquestacion-estados-compensacion/`. Un script manual reversible amplía exclusivamente las tablas modernas DOC-52 para conservar resultado y auditoría; no se ejecuta como parte del cambio.

## Risks / Trade-offs

- Storage usa sesión/temporales internamente; el adaptador limita, no elimina, ese riesgo.
- Persistir después de un efecto crea una ventana incierta que debe reconciliarse.
- La secuencia reduce throughput, pero preserva orden y recuperación.

## Migration Plan

1. Agregar puertos, estado y orquestador.
2. Caracterizar storage con dobles, sin almacenamiento real.
3. Ejecutar suite focal/build y auditar rutas legacy.
4. Retirar archivos nuevos como rollback.

## Open Questions

- Ninguna bloqueante; reconciliación externa completa queda para la entrega posterior.
