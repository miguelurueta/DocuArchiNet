# Flujo de integración

1. El usuario elige **Preparar** en una fila o selecciona varias y usa **Preparar seleccionados**.
2. La UI forma una colección exacta y muestra selectores desde `DocumentTypes` de capacidades.
3. Al completar tipologías llama una vez `PreflightImport`.
4. Backend valida contexto, colección, catálogo B09 y configuración B11 sin consultar SII.
5. La UI muestra tarea, tipología, requisitos y `EffectPlans` como previstos; nunca `ExpedientId`.
6. Confirmar envía colección, requisitos y `ContextFingerprint` a `CreateImportIntent` con clave idempotente.
7. La respuesta informa intención creada/reutilizada; DOC-75 no llama `ExecuteImportIntent`.
8. Cancelar/cerrar no muta y restaura scroll y foco.

El flujo individual y el batch usan el mismo contrato; solo cambia la cardinalidad.
