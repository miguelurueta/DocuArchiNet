# Rollout y rollback

## Riesgos y trazabilidad

| Requisito | Riesgo controlado | Prueba/evidencia | Estado |
|---|---|---|---|
| RQ-01 | arquitectura paralela | arquitectura local y este paquete | Local aprobado |
| RQ-02 | duplicados/reintentos | idempotencia, concurrencia local; E2E concurrente | E2E pendiente |
| RQ-03 | regresión legacy | invariancia de `ClassAlmacenamiento` | Local aprobado |
| RQ-04 | deriva contractual | contrato de ocho operaciones | Local aprobado |
| RQ-05 | cierre sin validación | suite, build, OpenSpec y OPSXJ | OPSXJ pendiente |
| RQ-06 | evidencia falsa/gate activo | runner común y restauración `finally` | Ejecución real aprobada; concurrencia pendiente |
| RQ-07 | autoridad del navegador, confusión ID TRD/checklist o salida inutilizable | autorización, traducción contextual, `ProjectExecutionResult` y contrato frontend | E2E RUP y salida directa aprobadas |
| RQ-08 | contrato SII ficticio/metadatos errados | sello real, clave opaca, matriz de índices y telemetría | E2E RUP aprobada; concurrencia pendiente |
