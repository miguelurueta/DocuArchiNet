# DOC-116 — verificación focal del adaptador de expedientes

## Why

El adaptador ya está implementado y DOC-111 acredita creación real en tres políticas. El ticket necesita cerrar combinaciones y negativos concretos, evitando otra implementación o repetir aceptación no afectada. La copia local del usuario es la fuente de revisión.

## What Changes

- Mapear evidencia por criterio y SHA local.
- Completar EXP-01: selección A→B conservando destino A, sin almacenar documentos.
- Caracterizar EXP-02 por ventana de fallo; separar descarte cliente y fallo interno real.
- Preparar EXP-03 sobre permiso nativo de nivel y actor no propietario, con restauración.
- Extender solo soporte/pruebas existentes ante una brecha comprobada; las rutas nuevas son propuestas.

## Capabilities

### New Capabilities

- `produccion-importacion-carga-carpeta-expediente`: contrato verificable de destino, autoridad y recuperación de la creación existente. Es una formalización de aceptación, no un segundo motor.

### Modified Capabilities

Ninguna modificación funcional de producto decidida en este corte.

## Impact

Adaptador/coordinador/repositorio existentes, arnés Framework y paquete E2E. No hay cambios de VB, configuración, esquema, almacenamiento o SII. Las pruebas reales exigen autorización propia; el refinamiento permanece en revisión.
