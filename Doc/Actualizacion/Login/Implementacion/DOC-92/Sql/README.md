# SQL DOC-92

DOC-92 no requiere migración: reutiliza exactamente la tabla compartida `docuarchi.ra_auth_second_factor_challenge` y sus columnas actuales:

`Id`, `ChallengeId`, `AuthUserId`, `Provider`, `CodeHash`, `ExpiresAtUtc`, `Consumed`, `Attempts`, `CreatedAtUtc` y `AuthPayloadJson`.

No se crean tablas, columnas, índices, procedimientos, eventos ni tareas de limpieza. En consecuencia, no existen scripts apply o rollback para esta entrega.

La compatibilidad se valida mediante pruebas estructurales y el harness MySQL descartable. Cualquier inspección de un ambiente real continúa requiriendo autorización explícita vigente.
