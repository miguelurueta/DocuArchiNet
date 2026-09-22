# Arquitectura

## Propiedad de ejecución

`ImportServiceOrchestrator` es el único ejecutor. El navegador crea la intención mediante el flujo DOC-75 y hace una sola llamada a `ExecuteImportIntent`; no ejecuta fases por elemento ni vuelve a consultar SII.

`ImportarServicioWebProgressAdapter` controla la promesa en vuelo, adapta el resultado estructurado y expone una lectura explícita de recuperación. `ImportarServicioWebProgressView` solo presenta espera y resultado. La UI los compone después de crear la intención.

## Flujo

1. El usuario confirma el plan preparado.
2. `CreateImportIntent` devuelve `IntentId` y `VersionToken`.
3. La vista anuncia espera global indeterminada.
4. El adaptador invoca una vez `ExecuteImportIntent` con la intención completa.
5. El adaptador transforma `Items`; la vista presenta cada elemento y el resumen.

La cardinalidad individual o múltiple no cambia esta secuencia. No hay polling, porcentaje, temporizador ni ejecución paralela desde el frontend.

Cerrar el modal oculta la presentación, pero no aborta ni revierte una ejecución iniciada. Una respuesta posterior se adapta normalmente. La recuperación de una intención conocida requiere una acción autorizada explícita.
