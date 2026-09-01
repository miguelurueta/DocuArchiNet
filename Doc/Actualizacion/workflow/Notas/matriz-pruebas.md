# Matriz de pruebas: Notas Workflow

| Cobertura | Mecanismo | Resultado esperado |
| --- | --- | --- |
| Contratos y cableado ASMX | `node --test tests/workflow-notes-contracts.test.cjs` | Contratos sin sesión, cursor protegido, SQL parametrizado, seis endpoints modernos, ETag canónico y preflight fail-closed. |
| Servicio con fakes | `WorkflowNotesReadBehaviorTests.exe` | Permiso ausente y cursor cruzado bloquean antes del repositorio; el listado autorizado conserva solo lecturas. |
| Repositorio con fakes | `WorkflowNotesReadRepositoryTests.exe` | Listado ordenado y paginado, contenido aislado y `COUNT(*)` con parámetros sin conexión MySQL. |
| Repositorio de escritura con fakes | `node --test tests/workflow-notes-write-repository.test.cjs` | Preflight incompatible o fallido responde `Unavailable` sin escritura; creación conserva libro de versión; reintento devuelve respuesta original; conflicto o borrado desactualizado no auditan y un fallo de auditoría revierte la transacción. |
| Plataforma E2E declarativa | `node --test tools/e2e/tests/workflow-e2e-platform-notes-write.test.cjs` | El perfil DOC-42 exige ambiente, ejecución y tarea descartable autorizados antes de solicitar secretos o abrir recursos. |
| Compilación VB.NET | `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal /nologo` | El proyecto compila sin errores. |
| E2E anónima | `npm.cmd --prefix tools/e2e run test:notes:anonymous` | El endpoint bloquea solicitudes sin sesión y no expone notas. |
| E2E autorizada de lectura | `npm.cmd --prefix tools/e2e run test:notes:read` | Conserva estado y auditoría, prueba listado, contenido y cursor inválido. |
| E2E autorizada de escritura | `npm.cmd --prefix tools/e2e run test:notes:write` y `test:notes:concurrency` | Sobre una tarea descartable autorizada, prueba creación idempotente, versión leída desde persistencia, actualización, conflicto, concurrencia, auditoría y borrado físico. |

## Condiciones de E2E

Antes de una corrida autenticada se debe leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. La persona responsable debe autorizar explícitamente el ambiente, la cuenta y la tarea de lectura. Para escritura o concurrencia, esa autorización debe incluir además una tarea descartable y permiso de ejecución; una autorización de lectura no basta. El arnés precarga la raíz local de Gestión, módulo y ambiente no sensibles, reutiliza el DSN ODBC no sensible `workflowconta` y pide en TTY solo las credenciales efímeras y la tarea; no acepta URL ni cadena de conexión MySQL. Registra las consultas de estado y auditoría, ambas `SELECT` de un parámetro, y no proyecta contenido de notas ni `datos_operacion`. La evidencia debe estar saneada y los gates locales deben permanecer apagados, sin usuarios ni grupos configurados.

Sin esas autorizaciones, la E2E se registra como bloqueo operacional. No se usa una base real, cuentas, secretos ni evidencia sintética para sustituirla.

## Ejecución local registrada

- `node --test tests/workflow-notes-contracts.test.cjs`: 8 pruebas aprobadas.
- `node --test tools/e2e/tests/notes-workflow-policy.test.cjs`: 6 pruebas aprobadas; valida la integración del ASMX sin iniciar Playwright.
- `WorkflowNotesReadBehaviorTests.exe`: aprobado con fakes de contexto, tarea, repositorio y cursor.
- `WorkflowNotesReadRepositoryTests.exe`: aprobado con conexión y ejecutor simulados.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m:1 /nologo`: compilación aprobada sin errores; el proyecto conserva advertencias históricas ajenas a DOC-41.

Para DOC-42, el 2026-09-01 se aprobaron 24 pruebas locales: 10 de contratos, 1 de repositorio de lectura, 1 de repositorio de escritura, 8 de política E2E y 4 de plataforma E2E declarativa. También aprobó `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal /nologo`, con advertencias históricas de ensamblados. Las pruebas locales no abren MySQL ni ejecutan etapas E2E mutantes.

En la E2E de escritura autorizada del 2026-09-01, la creación y su reintento idempotente confirmaron éxito después de satisfacer manualmente la precondición de esquema autorizada. La primera `ConsultarNota` posterior devolvió `Unavailable`, aislando la dependencia de `SHA2()` SQL usada sólo en lectura y mutación; la corrida se detuvo antes de actualizar o borrar. La corrección conserva el ETag SHA-256 calculado en .NET y exige `workflow_notas_version` InnoDB para su condición atómica. No se registrarán contenidos, identificadores, credenciales o cuerpos HTTP.

En una corrida posterior, la inspección confirmó que `workflow_notas_version` existe en InnoDB. Creación, reintento idempotente, consulta, actualización, conflicto y eliminación completaron sus aserciones funcionales. El fallo ocurrió únicamente al comparar la huella final de notas: el ciclo completo vuelve a dejarla igual que antes, mientras el arnés exigía erróneamente un cambio. La prueba ahora exige estado de notas sin cambio y auditoría con cambio, coherente con su registro declarativo. Hace falta una nueva tarea descartable y autorización independiente para confirmar la auditoría y generar evidencia final saneada.

La corrida siguiente de escritura autorizada pasó 1/1 en 24,5 s con la tabla de versiones disponible. Los controles de cierre confirmaron el gate apagado, usuarios y grupos vacíos y ausencia de cambios en WebForms legacy. La concurrencia permanece pendiente y requiere una tarea descartable distinta y autorización específica.

La primera carrera autorizada sobre una tarea descartable y nota semilla declaradas produjo `0/2` actualizaciones efectivas y quedó rechazada por el criterio de exactamente un éxito y un conflicto seguro. Antes de repetirla debe comprobarse por `SELECT` que la nota semilla tenga fila correspondiente en `workflow_notas_version`; las notas históricas sin ledger no son mutables por diseño DOC-42.

La carrera repetida con una nota semilla nueva produjo `1/2` actualizaciones efectivas y un conflicto de versión, con evidencia saneada. Los controles de cierre mantuvieron el gate apagado, usuarios y grupos vacíos y WebForms legacy sin cambios.

El 2026-08-31 se ejecutó la E2E local autorizada de Notas en GESTOR. El borde anónimo aprobó (1 caso, 737 ms) y la lectura autenticada de la tarea 708 aprobó (1 caso, 24.7 s): listado, consulta y cursor inválido conservaron las huellas de estado y auditoría. El certificado local autofirmado se aceptó solo mediante la excepción TLS efímera autorizada; el gate terminó apagado, las páginas legacy no cambiaron y la evidencia de lectura quedó saneada.
