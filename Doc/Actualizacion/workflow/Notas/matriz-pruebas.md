# Matriz de pruebas: lectura de Notas Workflow

| Cobertura | Mecanismo | Resultado esperado |
| --- | --- | --- |
| Contratos y cableado ASMX | `node --test tests/workflow-notes-contracts.test.cjs` | Contratos sin sesión, cursor protegido, SQL parametrizado y solo tres endpoints de lectura. |
| Servicio con fakes | `WorkflowNotesReadBehaviorTests.exe` | Permiso ausente y cursor cruzado bloquean antes del repositorio; el listado autorizado conserva solo lecturas. |
| Repositorio con fakes | `WorkflowNotesReadRepositoryTests.exe` | Listado ordenado y paginado, contenido aislado y `COUNT(*)` con parámetros sin conexión MySQL. |
| Compilación VB.NET | `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m:1 /nologo` | La solución compila sin errores. |
| E2E anónima | `npm.cmd --prefix tools/e2e run test:notes:anonymous` | El endpoint bloquea solicitudes sin sesión y no expone notas. |
| E2E autorizada de lectura | `npm.cmd --prefix tools/e2e run test:notes:read` | Conserva estado y auditoría, prueba listado, contenido y cursor inválido. |

## Condiciones de E2E

Antes de una corrida autenticada se debe leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. La persona responsable debe autorizar explícitamente el ambiente, la cuenta y la tarea de lectura. El arnés precarga la raíz local de Gestión, módulo y ambiente no sensibles, reutiliza el DSN ODBC no sensible `workflowconta` y pide en TTY solo las credenciales efímeras y la tarea; no acepta URL ni cadena de conexión MySQL. Registra las consultas de estado y auditoría, ambas `SELECT` de un parámetro, y no proyecta contenido de notas ni `datos_operacion`. La evidencia debe estar saneada y los gates locales deben permanecer apagados, sin usuarios ni grupos configurados.

Sin esas autorizaciones, la E2E se registra como bloqueo operacional. No se usa una base real, cuentas, secretos ni evidencia sintética para sustituirla.

## Ejecución local registrada

- `node --test tests/workflow-notes-contracts.test.cjs`: 8 pruebas aprobadas.
- `node --test tools/e2e/tests/notes-workflow-policy.test.cjs`: 6 pruebas aprobadas; valida la integración del ASMX sin iniciar Playwright.
- `WorkflowNotesReadBehaviorTests.exe`: aprobado con fakes de contexto, tarea, repositorio y cursor.
- `WorkflowNotesReadRepositoryTests.exe`: aprobado con conexión y ejecutor simulados.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m:1 /nologo`: compilación aprobada sin errores; el proyecto conserva advertencias históricas ajenas a DOC-41.

El 2026-08-31 se ejecutó la E2E local autorizada de Notas en GESTOR. El borde anónimo aprobó (1 caso, 737 ms) y la lectura autenticada de la tarea 708 aprobó (1 caso, 24.7 s): listado, consulta y cursor inválido conservaron las huellas de estado y auditoría. El certificado local autofirmado se aceptó solo mediante la excepción TLS efímera autorizada; el gate terminó apagado, las páginas legacy no cambiaron y la evidencia de lectura quedó saneada.
