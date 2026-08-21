## Why

DOC-28 incorporó los endpoints `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`, pero la automatización E2E disponible solo invoca los endpoints genéricos DOC-10/11. Por ello, la compilación y las pruebas estáticas no demuestran que el contrato específico de usuario funcione con una sesión Gestión, ni que su preview permanezca sin efectos.

## What Changes

- Incorporar un harness Playwright específico para el preview de envío a usuario, reutilizando el inicio de sesión Gestión existente.
- Añadir validaciones protegidas para los bordes anónimo, parámetros inválidos y preview autenticado de solo lectura, con huellas de estado y auditoría antes/después.
- Preparar, sin ejecutar por defecto, una prueba de ejecución sobre una tarea descartable que exija autorización explícita y controles SQL de solo lectura.
- Documentar los prerrequisitos, las variables efímeras y la evidencia mínima para ejecutar las pruebas en un ambiente autorizado.

## Capabilities

### New Capabilities

- `e2e-enviar-usuario-workflow`: Cobertura E2E segura y explícitamente autorizada para los endpoints de envío a usuario del workflow.

### Modified Capabilities

- Ninguna.

## Impact

- `tools/e2e/tests/`, `tools/e2e/scripts/`, `tools/e2e/package.json` y su documentación operativa.
- No se modifica el contrato ASMX, el flujo legacy ni la configuración del gate.
- Las corridas reales requieren URL de pruebas, cuentas entregadas por secretos efímeros, acceso MySQL de solo lectura y, para ejecución, una tarea descartable autorizada.
