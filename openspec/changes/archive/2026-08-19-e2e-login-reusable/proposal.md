## Why

Las pruebas autenticadas repiten el inicio de sesión Web Forms, lo que vuelve más frágil cada prueba nueva y aumenta el riesgo de manejar secretos de forma inconsistente. Se necesita una única utilidad reutilizable que conserve el bootstrap de sesión y mantenga las credenciales fuera de código, evidencias y registros.

## What Changes

- Añadir un helper de Playwright para autenticar una sesión de Gestión Documental a partir de variables de entorno de la sesión.
- Centralizar la selección de módulo, el postback Web Forms, la espera de autenticación y el cierre seguro del contexto.
- Reutilizar el helper en las suites E2E existentes y documentar su contrato, sin ejecutar pruebas autenticadas como parte de esta implementación.

## Capabilities

No se modifican requisitos funcionales del producto; es una refactorización de tooling de pruebas. La especificación se omite mediante `skip_specs: true`.

### New Capabilities

Ninguna.

### Modified Capabilities

Ninguna.

## Impact

- `tools/e2e/tests/` y sus utilidades compartidas.
- Scripts y documentación de E2E, si requieren referenciar el helper.
- No se modifican endpoints, contratos de Workflow, datos, gates ni dependencias de producción.
