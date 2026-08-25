## Why

La modernización de Notas de Workflow requiere validación contractual integrada y debe conservar la ejecución E2E oficial de la transición Workflow en DOC-32. Las pruebas unitarias no demuestran la reconstrucción de sesión Gestión→Workflow, el aislamiento de tareas ni las huellas reales de auditoría. El proyecto ya dispone de un arnés E2E protegido que debe reutilizarse para evitar nuevos logins, secretos persistidos o mecanismos de prueba divergentes.

## What Changes

- Incorporar validaciones contractuales de Notas de Workflow dentro de `tools/e2e`, reutilizando la sesión autenticada y configuración Playwright existente.
- Mantener la ejecución E2E real de transición exclusivamente en los comandos DOC-32 (`preview`, `execute` y `concurrency`), sin crear una ejecución paralela para Notas.
- Incorporar validadores y comandos npm para separar borde anónimo, lectura real no mutante y escrituras contractuales explícitamente autorizadas sobre tareas descartables.
- Hacer que los comandos de Notas y DOC-32 soliciten la configuración necesaria desde la consola interactiva y la entreguen únicamente a sus procesos hijos, para no exigir una carga manual de variables de entorno.
- Verificar por huellas de consultas MySQL de solo lectura que el listado no altera estado ni auditoría, y que las operaciones mutantes autorizadas producen el resultado y la auditoría esperados.
- Generar evidencia saneada sin credenciales, cookies, contenido de notas, destinos ni cuerpos de respuesta.
- Mantener el gate de Centro de Trabajo apagado y no cambiar el recorrido legacy como consecuencia de las pruebas.

## Capabilities

### New Capabilities

- `e2e-notas-workflow`: Validación contractual protegida de Notas y cierre E2E de transición mediante DOC-32.

### Modified Capabilities

- Ninguna.

## Impact

- Afecta `tools/e2e/package.json`, pruebas Playwright, scripts de validación de configuración, comandos DOC-32 y artefactos de evidencia saneada.
- Reutiliza `tools/e2e/tests/support/authenticated-workflow-session.cjs`, `playwright.config.cjs`, MySQL en modo solo lectura y las convenciones de las suites DOC-28/DOC-32.
- Depende de los contratos modernos de Notas ya implementados y del ambiente autorizado, cuentas de prueba y tareas descartables de DOC-32 para ejecutar la transición real.
