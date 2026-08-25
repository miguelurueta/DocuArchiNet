## Why

Las corridas E2E de Workflow repiten la captura de la misma configuración y secretos para preview, ejecución y concurrencia. Esto añade fricción, dificulta su ejecución asistida desde Codex y aumenta el riesgo de configuraciones inconsistentes entre DOC.

## What Changes

- Incorporar un orquestador único y configurable para ejecutar una secuencia E2E Workflow autorizada con una única sesión de configuración efímera.
- Permitir perfiles locales persistentes únicamente para claves no sensibles y validadas, reutilizables por DOC y por ambiente.
- Mantener fuera de los perfiles toda contraseña, cookie, token, cadena de conexión y confirmación de autorización; esos datos se reciben de forma efímera y no se imprimen ni persisten.
- Centralizar las validaciones de perfil, las confirmaciones explícitas por operación mutante, los presupuestos de latencia, los controles de cierre y la evidencia saneada.
- Conservar los comandos específicos actuales durante la transición, y habilitar DOC-32 como primer consumidor del orquestador reutilizable.

## Capabilities

### New Capabilities

- `orquestador-e2e-workflow-reutilizable`: Ejecuta secuencias E2E Workflow autorizadas desde un perfil no sensible y una sesión efímera de secretos, con contratos configurables por DOC.

### Modified Capabilities

- `e2e-enviar-usuario-workflow`: Alinear las garantías comunes de autorización, secretos efímeros, evidencia saneada y cierre con el orquestador reutilizable.

## Impact

- `tools/e2e/package.json`, scripts de consola interactiva y comprobaciones de configuración.
- Pruebas de política y de contrato E2E para DOC-32 y la infraestructura compartida.
- `tools/e2e/AGENT-RUNBOOK.md` y documentación técnica de los DOC consumidores.
- Perfiles externos locales, como `C:\cert\contet.txt`, leídos sin ser copiados al repositorio ni a los artefactos.
