## Trazabilidad

- D-01: suite DOC-28 independiente con modos separados.
- D-02: ejecución derivada de un preview actual.
- D-03: validación fail-closed antes de navegador o HTTP.
- D-04: evidencia sanitizada e invariantes locales.
- RQ-01: Preview E2E específico y seguro.
- RQ-02: Prueba de preview sin mutación verificable.
- RQ-03: Ejecución E2E explícitamente autorizada.
- RQ-04: Evidencia libre de secretos y cierre seguro.

## 0. Corrección de inclusión de proyecto

- [x] 0.1 [S] Verificar las inclusiones de proyecto de los tipos exclusivos de envío a usuario y protegerlas contra regresiones. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`, `tests/workflow-user-send.test.cjs`. Origen: D-01, RQ-01, RQ-03. Verificación: la reconstrucción Debug de la solución pasa y la prueba estática exige cada entrada `<Compile Include>` exclusiva de usuario.

## 1. Configuración protegida

- [x] 1.1 [M] Validar los prerrequisitos DOC-28 por modo antes de abrir Playwright o enviar HTTP. Área/archivos: `tools/e2e/scripts/assert-doc28-user-send-config.cjs`. Origen: D-03, RQ-01, RQ-02, RQ-03. Verificación: un proceso local sin variables requeridas termina con error sanitizado y el modo de ejecución rechaza cualquier autorización distinta de `true`.
- [x] 1.2 [S] Registrar comandos npm DOC-28 separados para anónimo, validación, preview completo y ejecución autorizada. Área/archivos: `tools/e2e/package.json`. Origen: D-01, D-03, RQ-01, RQ-02, RQ-03. Verificación: cada comando invoca primero el validador DOC-28 y selecciona únicamente su etiqueta Playwright.
- [x] 1.3 [S] Cubrir localmente las barreras de configuración y el aislamiento del contrato DOC-28 sin usar credenciales ni URL remota. Área/archivos: `tests/workflow-user-send-e2e.test.cjs`. Origen: D-01, D-03, RQ-01, RQ-03. Verificación: `node --test tests/workflow-user-send-e2e.test.cjs` confirma endpoints, payload explícito, autorización exacta y ausencia de variables de destino/token configurables.

## 2. Preview de usuario sin mutación

- [x] 2.1 [M] Implementar el borde anónimo del preview de envío a usuario con envelope JSON sanitizado y cero destinos al bloquearse. Área/archivos: `tools/e2e/tests/doc28-user-send.spec.cjs`. Origen: D-01, RQ-01. Verificación: la etiqueta anónima exige URL, recibe HTTP 200 con bloqueo de contexto y no expone destinos ni detalles internos.
- [x] 2.2 [M] Implementar la validación autenticada de parámetros de preview usando el helper Gestión compartido. Área/archivos: `tools/e2e/tests/doc28-user-send.spec.cjs`. Origen: D-01, RQ-01. Verificación: la etiqueta de validación usa una sesión Gestión y comprueba el código funcional para solicitud inválida sin transición ni texto SQL/excepción.
- [x] 2.3 [M] Implementar el preview completo con controles `SELECT` de un parámetro, paginación verificable, huellas antes/después y evidencia resumida. Área/archivos: `tools/e2e/tests/doc28-user-send.spec.cjs`. Origen: D-01, D-04, RQ-02, RQ-04. Verificación: en ambiente autorizado, el comando de preview produce huellas iguales de estado/auditoría y un JSON sin secretos; localmente, la prueba de fuente verifica la validación SQL y los campos permitidos de evidencia.

## 3. Ejecución controlada

- [x] 3.1 [M] Implementar la prueba de ejecución que obtiene token y destino del preview actual y mide las huellas de la tarea descartable. Área/archivos: `tools/e2e/tests/doc28-user-send.spec.cjs`. Origen: D-02, D-04, RQ-03, RQ-04. Verificación: en una corrida explícitamente autorizada se envía exactamente el destino/token del preview y se valida el resultado y las huellas esperadas; sin autorización, el validador falla antes de la sesión.
- [x] 3.2 [S] Mantener la ejecución y cualquier posible concurrencia fuera de comandos de preview y sin activación de gate. Área/archivos: `tools/e2e/package.json`, `tools/e2e/tests/doc28-user-send.spec.cjs`. Origen: D-01, D-03, D-04, RQ-03, RQ-04. Verificación: las pruebas estáticas confirman que no existe comando de carga DOC-28, que la ejecución tiene etiqueta exclusiva y que ninguna prueba escribe SQL ni edita configuración.

## 4. Documentación y evidencia

- [x] 4.1 [S] Documentar los comandos DOC-28, prerrequisitos efímeros, autorización de mutación y cierre de invariantes. Área/archivos: `tools/e2e/README.md`, `tools/e2e/AGENT-RUNBOOK.md`. Origen: D-03, D-04, RQ-02, RQ-03, RQ-04. Verificación: la guía distingue preview sin mutación de ejecución descartable, prohíbe secretos versionados y exige gate apagado/listas vacías al cierre.
- [x] 4.2 [S] Ejecutar y registrar la validación local sin autenticación de la nueva suite y de la regresión focalizada DOC-28. Área/archivos: `tests/workflow-user-send-e2e.test.cjs`, `tests/workflow-user-send.test.cjs`, `tools/e2e/scripts/assert-doc28-user-send-config.cjs`. Origen: D-01, D-03, D-04, RQ-01, RQ-02, RQ-03, RQ-04. Verificación: `node --test` de ambas pruebas pasa y los validadores incompletos fallan sin iniciar navegador, red ni base de datos.
- [x] 4.3 [M] Ejecutar y registrar la evidencia E2E DOC-28 en el ambiente de pruebas explícitamente identificado. Área/archivos: `tools/e2e/tests/doc28-user-send.spec.cjs`, `tools/e2e/artifacts/` (ignorado). Origen: D-01, D-02, D-03, D-04, RQ-01, RQ-02, RQ-03, RQ-04. Verificación: con URL, cuentas de prueba, consultas MySQL solo lectura y tarea descartable autorizadas por el responsable del ambiente, se completan los modos anónimo/validación/preview y, solo si se autoriza la mutación, ejecución; la evidencia conserva únicamente códigos, conteos y huellas, y el gate queda apagado con listas vacías.
