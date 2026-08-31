## 1. Contratos declarativos

- [x] 1.1 [M] Crear el registro cerrado de escenarios, controles y adaptadores de la plataforma. Área/archivos: `tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs`. Origen: D-02, RQ-01, RQ-05. Verificación: una prueba unitaria resuelve únicamente identificadores registrados y rechaza uno desconocido.
- [x] 1.2 [M] Crear el cargador y validador estricto de perfiles no sensibles. Área/archivos: `tools/e2e/scripts/support/workflow-e2e-platform-profile.cjs`, `tools/e2e/profiles/workflow-e2e-platform.profile.example.json`. Origen: D-02, D-03, RQ-01, RQ-04. Verificación: una prueba rechaza campos desconocidos, secretos, SQL y URL/cadenas de conexión de base de datos sin revelar su valor.
- [x] 1.3 [S] Declarar el adaptador de lectura de Notas con sus operaciones y expectativas permitidas. Área/archivos: `tools/e2e/scripts/adapters/notes-read-e2e-adapter.cjs`. Origen: D-02, D-06, RQ-05. Verificación: una prueba de contrato confirma que expone listado, consulta y cursor inválido, sin importar infraestructura prohibida.

## 2. Kernel y ciclo seguro

- [x] 2.1 [M] Implementar el preflight común que valida perfil, escenario, etapa y autorizaciones antes de abrir recursos externos. Área/archivos: `tools/e2e/scripts/support/workflow-e2e-platform.cjs`. Origen: D-01, D-03, D-04, RQ-01, RQ-02. Verificación: una prueba demuestra que un perfil o autorización inválidos no construyen sesión ni cliente ODBC.
- [x] 2.2 [M] Implementar la reserva, controles antes/después, bloqueo de etapas posteriores y cierre obligatorio del ciclo común. Área/archivos: `tools/e2e/scripts/support/workflow-e2e-platform.cjs`, `tools/e2e/scripts/support/e2e-test-resource-lifecycle.cjs`. Origen: D-04, RQ-02, RQ-03. Verificación: una prueba de orden demuestra controles de no mutación y cierre aun cuando la etapa falla.
- [x] 2.3 [M] Centralizar el transporte autenticado, la excepción TLS local efímera y la evidencia saneada. Área/archivos: `tools/e2e/scripts/support/workflow-e2e-platform.cjs`, `tools/e2e/scripts/support/interactive-e2e-console.cjs`. Origen: D-03, D-05, RQ-04. Verificación: pruebas confirman TLS estricto por defecto, excepción autorizada consistente y ausencia de datos sensibles en la evidencia.
- [x] 2.4 [S] Exponer el ejecutor de plataforma mediante un comando npm adicional y argumentos limitados. Área/archivos: `tools/e2e/scripts/run-workflow-e2e-platform.cjs`, `tools/e2e/package.json`. Origen: D-01, D-06, RQ-05. Verificación: el comando rechaza argumentos no registrados antes de iniciar una corrida.

## 3. Piloto de lectura de Notas

- [x] 3.1 [M] Implementar la etapa declarativa `notes-read` usando la sesión y los controles aprobados existentes. Área/archivos: `tools/e2e/scripts/adapters/notes-read-e2e-adapter.cjs`, `tools/e2e/scripts/support/notes-workflow-e2e.cjs`. Origen: D-04, D-06, RQ-03, RQ-05. Verificación: una prueba simulada compara huellas antes/después y valida listado, consulta y cursor inválido sin conservar respuestas.
- [x] 3.2 [S] Mantener compatible el arnés y los comandos actuales de Notas durante el piloto. Área/archivos: `tools/e2e/package.json`, `tools/e2e/tests/notes-workflow-policy.test.cjs`. Origen: D-06, RQ-05. Verificación: la prueba de política comprueba que `test:notes:read` continúa apuntando al arnés existente.

## 4. Cobertura automatizada

- [x] 4.1 [M] Cubrir las reglas de registro y perfil de la plataforma. Área/archivos: `tools/e2e/tests/workflow-e2e-platform-registry.test.cjs`, `tools/e2e/tests/workflow-e2e-platform-profile.test.cjs`. Origen: D-02, D-03, RQ-01, RQ-04. Verificación: `node --test` completa los casos válidos y de rechazo sin conexiones externas.
- [x] 4.2 [M] Cubrir el orden de ciclo, recursos y cierre obligatorio del kernel. Área/archivos: `tools/e2e/tests/workflow-e2e-platform.test.cjs`. Origen: D-04, RQ-02, RQ-03. Verificación: `node --test` confirma reserva/liberación y cierre cuando preflight, controles o etapa fallan.
- [x] 4.3 [M] Cubrir las fronteras de secretos, TLS, artefactos temporales y evidencia. Área/archivos: `tools/e2e/tests/workflow-e2e-platform-security.test.cjs`. Origen: D-03, D-05, RQ-04. Verificación: `node --test` verifica que los valores sensibles no llegan a salida ni a evidencia y que el temporal se elimina.
- [x] 4.4 [M] Cubrir el contrato y la equivalencia funcional simulada del adaptador `notes-read`. Área/archivos: `tools/e2e/tests/notes-read-e2e-adapter.test.cjs`. Origen: D-06, RQ-03, RQ-05. Verificación: `node --test` valida operaciones permitidas, controles registrados y no mutación.

## 5. Documentación y validación

- [x] 5.1 [S] Documentar el contrato, perfil, comando y migración de un DOC en la guía reutilizable. Área/archivos: `tools/e2e/E2E-TEST/IMPLEMENTATION-PROMPT.md`, `tools/e2e/E2E-TEST/AGENTS.md`. Origen: D-01, D-02, D-06, RQ-01, RQ-05. Verificación: la guía permite declarar un adaptador sin secretos, SQL ni comandos arbitrarios.
- [x] 5.2 [S] Validar la implementación local y documentar la condición de autorización para E2E reales. Área/archivos: `openspec/changes/e2e-test-plataforma-reutilizable/tasks.md`. Origen: D-04, D-05, RQ-02, RQ-03, RQ-04. Verificación: `openspec validate e2e-test-plataforma-reutilizable --strict` y las pruebas automatizadas de plataforma terminan correctamente sin iniciar una E2E real.
