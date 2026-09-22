# Contrato y mapping

## Entrada común

Cada `ImportItemSelectionDto` incluye `ClientItemId`, `ExternalKey`, `TargetTaskId`, `DocumentTypeId`, `DocumentTypeName`, nombre y MIME. `DocumentTypeId` solo se acepta si existe en el catálogo autorizado B09.

## Preflight B03/B11

`PreflightImport` recibe `Items` y devuelve `Executable`, `Requirements`, `Commands`, `ContextFingerprint` y `EffectPlans`. El frontend conserva estos valores, no los calcula. Un plan contiene tarea, tipología, modo de destino, requisitos y efectos con estado previsto.

## Creación

`CreateImportIntent` recibe una sola colección completa, `IdempotencyKey`, `Requirements`, huella y radicado. Backend repite preflight y responde `PREFLIGHT_STALE` si el contexto cambió. La deduplicación cliente comparte la promesa; la idempotencia real pertenece al backend.
