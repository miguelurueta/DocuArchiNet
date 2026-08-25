## 1. Configuración protegida de la suite

- [x] 1.1 Implementar el validador `assert-notes-workflow-config.cjs` con modos anónimo, lectura, escritura y concurrencia; validar URL, enteros, autorizaciones explícitas y consultas `SELECT` de un parámetro sin mostrar valores sensibles.
- [x] 1.2 Registrar los comandos `test:notes:anonymous`, `test:notes:read`, `test:notes:write` y `test:notes:concurrency` en `tools/e2e/package.json`, encadenando el validador antes de Playwright o de la carrera fija.
- [x] 1.3 Reemplazar la carga manual por un iniciador interactivo TTY que capture la configuración de cada modo de forma efímera, oculte los secretos, ejecute los hijos con ese entorno y falle de forma segura sin TTY.
- [x] 1.4 Aplicar el mismo iniciador efímero a `test:doc32:*`, para que la ejecución oficial de transición DOC-32 no requiera variables cargadas manualmente.

## 2. Suite E2E de lectura y controles comunes

- [x] 2.1 Crear `notes-workflow.spec.cjs` reutilizando la sesión autenticada, Playwright, comprobación de gate, verificación de páginas legacy, DTO público, SQL de solo lectura, huellas y evidencia saneada de DOC-32.
- [x] 2.2 Implementar los casos anónimo y de lectura real: bloqueo sin sesión, listado/consulta autorizados, paginación/orden y aislamiento de tarea o cursor, con huellas de estado y auditoría inmutables.

## 3. Escritura autorizada y concurrencia acotada

- [x] 3.1 Implementar el caso de escritura real, protegido por autorización de tarea descartable, que valida crear idempotente, actualización, conflicto de versión, eliminación según política aprobada y huellas/auditoría esperadas.
- [x] 3.2 Implementar un ejecutor de concurrencia de exactamente dos solicitudes para Notas, protegido por autorización y tarea descartable distinta, que verifica un único cambio válido y un bloqueo o conflicto seguro.

## 4. Documentación y verificación

- [x] 4.1 Extender `tools/e2e/AGENT-RUNBOOK.md` con preparación, variables efímeras, comandos, autorizaciones, evidencia y cierre específicos de Notas, sin debilitar sus límites existentes.
- [x] 4.2 Añadir o actualizar pruebas unitarias estáticas del arnés para el validador, uso obligatorio de la sesión compartida, saneamiento de evidencia y ausencia de login paralelo.
- [x] 4.3 Ejecutar las pruebas locales no autenticadas y los validadores de configuración; registrar sus resultados sin secretos.
- [x] 4.4 Ejecutar `test:doc32:preview`, `test:doc32:execute` y `test:doc32:concurrency` reales con ambiente, cuentas, MySQL de solo lectura y tareas descartables explícitamente autorizados; si falta cualquiera, registrar el bloqueo sin marcar esta tarea como completa ni sustituirla por simulación.
