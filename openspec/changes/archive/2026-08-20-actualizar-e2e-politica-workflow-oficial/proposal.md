## Why

La E2E completa todavía modela el gate de piloto/no piloto retirado de la experiencia oficial. Ejecutarla así produciría falsos fallos y podría volver a normalizar un bloqueo que ya no debe existir.

## What Changes

- Sustituir las aserciones de piloto/no piloto y `WORKFLOW_MODERN_INACTIVE` por comprobaciones de contexto Workflow válido, respuesta funcional y ausencia de mutación.
- Conservar el helper autenticado existente y prohibir que las pruebas alteren el gate o dependan de listas de usuarios/grupos.
- Actualizar configuración, documentación y evidencia E2E para que describan la política moderna oficial y sus límites de solo lectura.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

Ninguna. Este cambio ajusta pruebas y documentación; no modifica el comportamiento del producto.

## Impact

- `tools/e2e/tests/doc10-preview.spec.cjs` y su validación de variables.
- Runbook y README de E2E, junto con la evidencia de QA correspondiente.
- No modifica endpoints, Web Forms, gates ni permisos de negocio.
